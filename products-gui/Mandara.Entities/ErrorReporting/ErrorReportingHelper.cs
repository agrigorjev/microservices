using System;
using System.Collections.Generic;
using Mandara.Common.Logging;
using Mandara.Entities.Calculation;
using NLog;

namespace Mandara.Entities.ErrorReporting
{
    public static class ErrorReportingHelper
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        public static MessageQueue GlobalQueue = new MessageQueue();

        public static void EnqueueCalculationErrors(
            List<CalculationError> calculationErrors,
            String source,
            ErrorLevel level = ErrorLevel.Critical)
        {
            if (calculationErrors != null && calculationErrors.Count > 0)
            {
                foreach (CalculationError error in calculationErrors)
                {
                    GlobalQueue.Enqueue(
                        new Error(
                            source,
                            ErrorType.CalculationError,
                            error.ErrorMessage,
                            null,
                            error.SourceDetail,
                            level)
                        {
                            UserName = error.UserName
                        });
                }
            }
        }        

        public static void ReportError(Error err)
        {
            ReportError(err.UserName, err.Source, err.Type, err.Message, err.ObjectId, err.Object, err.Level);
        }

        public static void ReportError(
            string userName,
            String source,
            ErrorType errorType,
            String message,
            String objectId = null,
            Object o = null,
            ErrorLevel level = ErrorLevel.Normal)
        {
            LogError(
                userName,
                source,
                errorType,
                message,
                objectId,
                o,
                level);

            GlobalQueue.Enqueue(
                new Error(
                    userName,
                    source,
                    errorType,
                    message,
                    objectId,
                    o,
                    level));
        }

        public static void ReportError(
            String source,
            ErrorType errorType,
            String message,
            String objectId = null,
            Object o = null,
            ErrorLevel level = ErrorLevel.Normal)
        {
            LogError(
                "",
                source,
                errorType,
                message,
                objectId,
                o,
                level);

            GlobalQueue.Enqueue(
                new Error(
                    source,
                    errorType,
                    message,
                    objectId,
                    o,
                    level));
        }

        public static void Close()
        {
            GlobalQueue.Dispose();
        }

        private static void LogError(
            string userName,
            string source,
            ErrorType errorType,
            string message,
            string objectId,
            object o,
            ErrorLevel level)
        {
            if (level != ErrorLevel.Critical || errorType == ErrorType.TradeOnHolidayDate)
                return;

            if (o is Exception ex)
            {
                _logger.Error(
                    ex,
                    "user {0} source {1} errorTye {2} message {3} objectId {4} level {5} ",
                    userName,
                    source,
                    errorType,
                    message,
                    objectId,
                    level);
            }
            else
            {
                _logger.Error(
                    "user {0} source {1} errorTye {2} message {3} objectId {4} level {5} object {6} ",
                    userName,
                    source,
                    errorType,
                    message,
                    objectId,
                    level,
                    o);
            }
        }
    }
}