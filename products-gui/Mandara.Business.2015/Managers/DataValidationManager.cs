using Mandara.Business.Bus;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Mandara.Entities.ErrorReporting;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Mandara.Business
{
    public class DataValidationManager
    {
        private DateTime _lastTimestamp = SystemTime.Now();
        private DateTime _lastTradeSupportCheckTimestamp = DateTime.MinValue;

        public List<Error> GetTradeTransferErrors()
        {
            List<TradeTransferError> errors = GetTransferErrors(tte => tte.ErrorDate > _lastTimestamp);
            if (errors.Any())
            {
                _lastTimestamp = errors.Max(tte => tte.ErrorDate);
            }
            return CreateErrors(errors);
        }

        /// <summary>
        /// Enlists trade transfer errors to display in trade support log
        /// </summary>
        /// <returns>Collected errors</returns>
        public List<Error> GetTradeTransferErrorsForTradeSupport()
        {
            List<TradeTransferError> errors = GetTransferErrors(
                transErr => ((transErr.ErrorDate > _lastTradeSupportCheckTimestamp)
                             && ((transErr.ErrorTypeDb == (short)TransferErrorType.UnknownProductAlias)
                                 || (transErr.ErrorTypeDb == (short)TransferErrorType.TradeOnHolidayDate))));

            if (errors.Any())
            {
                _lastTradeSupportCheckTimestamp = errors.Max(transErr => transErr.ErrorDate);
            }

            return CreateSupportErrors(errors);
        }

        public static List<Error> GetAllTradeTransferErrors()
        {
            List<TradeTransferError> errors = GetTransferErrors(t => true);
            return CreateErrors(errors);
        }

        private static List<TradeTransferError> GetTransferErrors(Expression<Func<TradeTransferError, bool>> predicate)
        {
            try
            {
                using (MandaraEntities cxt = new MandaraEntities(
                    MandaraEntities.DefaultConnStrName,
                    nameof(DataValidationManager)))
                {
                    return cxt.TradeTransferErrors.Where(predicate).OrderBy(tc => tc.ErrorDate).AsNoTracking().ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorReportingHelper.GlobalQueue.Enqueue(
                    new Error(
                        "Data Validation",
                        ErrorType.Exception,
                        "Cannot retrieve trade transfer errors from database.",
                        null,
                        ex,
                        ErrorLevel.Critical));
                return new List<TradeTransferError>();
            }
        }

        private static List<Error> CreateSupportErrors(List<TradeTransferError> transferErrors)
        {
            List<TradeTransferError> supportErrors = transferErrors
                ?.Where(
                    error => (error.ErrorType == TransferErrorType.UnknownProductAlias)
                             || (error.ErrorType == TransferErrorType.TradeOnHolidayDate))
                .ToList();

            return CreateErrors(supportErrors);
        }

        private static List<Error> CreateErrors(List<TradeTransferError> transferErrors)
        {
            return transferErrors?.OrderBy(error => error.ErrorDate)
                                 .GroupBy(error => error.EntityId)
                                 .Select(errorsByEntity => CreateError(errorsByEntity.First()))
                                 .ToList();
        }

        public static Error CreateError(TradeTransferError error)
        {
            string errorMessage;
            object details = null;
            bool handleBySupport = false;
            ErrorType errorType = ErrorType.DataError;

            switch (error.ErrorType)
            {
                case TransferErrorType.UnknownProductAlias:
                {
                    details = GetTransferErrorDetails(error);
                    errorMessage = GetErrorMessageForUnknownProductAlias(
                        error.ErrorMessage,
                        new TryGetRef<TradeTransferErrorDetails>() { Value = details as TradeTransferErrorDetails });
                    handleBySupport = true;
                }
                break;

                case TransferErrorType.UnknownStripName:
                {
                    errorMessage =
                        $"DataValidationManager - SecurityDefinition error: unknown strip name '{error.ErrorMessage}'.";
                }
                break;

                case TransferErrorType.UnknownSecurityDefinition:
                {
                    errorMessage = "DataValidationManager - Trade error: unknown security definition.";
                    handleBySupport = true;
                }
                break;

                case TransferErrorType.TradeTransferFailure:
                {
                    errorMessage = "DataValidationManager - Trade error: exception during transfer, see details.";
                    details = error.ExceptionDetail;
                }
                break;

                case TransferErrorType.UnknownPortfolio:
                {
                    errorMessage = "DataValidationManager - Portfolio cannot be identified for a "
                                   + "trade. The trade is moved to the error book.";
                }
                break;

                case TransferErrorType.TradeOnHolidayDate:
                {
                    errorType = ErrorType.TradeOnHolidayDate;
                    errorMessage =
                        $"DataValidationManager - Trade executed on holiday day, trade details [{error.ErrorMessage}].";
                    handleBySupport = true;
                }
                break;

                default:
                {
                    errorType = ErrorType.Unknown;
                    errorMessage = $"DataValidationManager - Unknown error type: {error.ErrorMessage}";
                    handleBySupport = true;
                }
                break;
            }

            return new Error(
                "Data Validation",
                errorType,
                errorMessage,
                error.EntityId.ToString(),
                details,
                ErrorLevel.Critical,
                handleBySupport);
        }

        private static object GetTransferErrorDetails(TradeTransferError error)
        {
            return JsonHelper.DeserializeQuiet<TradeTransferErrorDetails>(error.ErrorMessage);
        }

        private static string GetErrorMessageForUnknownProductAlias(
            string defaultErrorMsg,
            TryGetResult<TradeTransferErrorDetails> errorDetails)
        {
            string errorMessage;

            if (errorDetails.HasValue)
            {
                errorMessage = "DataValidationManager - SecurityDefinition error: "
                               + $"unknown product alias '{errorDetails.Value.Product}'.";
            }
            else
            {
                errorMessage = defaultErrorMsg;
            }

            return errorMessage;
        }
    }
}