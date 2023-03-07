using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Ninject.Extensions.Logging;
using System;
using Mandara.Date;

namespace Mandara.Business.Extensions
{
    public static class ProductExtensions
    {
        public const int CalendarDaySwapDaysOffsetFromFriday = 2;
        public static bool HasRolledOff(this Product product, DateTime? riskDate = null)
        {
            riskDate =  riskDate ?? SystemTime.Now();

            if (product.RolloffTime.HasValue && product.TasType == TasType.NotTas)
            {
                DateTime productRiskDate = new DateTime(product.RolloffTime.Value.Year, product.RolloffTime.Value.Month, product.RolloffTime.Value.Day, product.RolloffTime.Value.Hour, product.RolloffTime.Value.Minute, product.RolloffTime.Value.Second, DateTimeKind.Unspecified);

                productRiskDate = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(productRiskDate,
                    product.TimezoneId, TimeZoneInfo.Local.Id);

                return riskDate.Value > productRiskDate;
            }

            return false;

            // if (product.Type == ProductType.Futures && product.FuturesExpireTime.HasValue) { productRiskDate =
            // productRiskDate.Subtract(product.FuturesExpireTime.Value.TimeOfDay); }
            //
            // return productRiskDate;
        }

        public static DateTime RiskDate(this Product product, DateTime riskDate)
        {
            DateTime productRiskDate = DateTime.SpecifyKind(riskDate, DateTimeKind.Unspecified);

            if (product.Type == ProductType.Futures && product.FuturesExpireTime.HasValue)
            {
                productRiskDate = productRiskDate.Subtract(product.FuturesExpireTime.Value.TimeOfDay);
            }

            if (product.RolloffTime.HasValue && (product.TasType == TasType.NotTas || product.IsMoC()))
            {
                productRiskDate = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                                                  productRiskDate,
                                                  TimeZoneInfo.Local.Id,
                                                  product.TimezoneId)
                                              .Subtract(product.RolloffTime.Value.TimeOfDay);
            }

            if (product.IsCalendarDaySwap && productRiskDate.DayOfWeek == DayOfWeek.Friday)
            {
                productRiskDate = productRiskDate.Date.AddDays(CalendarDaySwapDaysOffsetFromFriday);
            }

            return productRiskDate;
        }

        public static bool IsMoC(this Product product) =>
            ProductType.Swap == product.Type && TasType.Tas == product.TasType;

        public static DateTime GetRiskDate(this Product product, DateTime? riskDate = null) =>
            product.RiskDate(riskDate ?? SystemTime.Now());

        public static DateTime GetTasActivationTime(this Product product, DateTime? riskDate = null)
        {
            DateTime productRiskDate = DateTime.SpecifyKind(riskDate ?? InternalTime.LocalNow(), DateTimeKind.Local);

            if (product.RolloffTime.HasValue)
            {
                productRiskDate = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                    productRiskDate,
                    TimeZoneInfo.Local.Id,
                    product.TimezoneId);

                productRiskDate = productRiskDate.Subtract(product.RolloffTime.Value.TimeOfDay);
            }

            return productRiskDate;
        }

        public static DateTime ConvertTransactTimeToUtc(DateTime transactTime, Product product, ILogger log)
        {
            try
            {
                string timeZone = product.Exchange.TimeZoneId;
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                return TimeZoneInfo.ConvertTimeToUtc(
                    DateTime.SpecifyKind(transactTime, DateTimeKind.Unspecified),
                    timeZoneInfo);
            }
            catch (Exception ex)
            {
                string errorMessage = "Error occured when attempting to convert TransactTime to UtcTransactTime.";

                if (product == null)
                {
                    errorMessage =
                        String.Format("{0} Null Product given. Returning UTC current time instead", errorMessage);
                }
                else if (product.Exchange == null)
                {
                    errorMessage = String.Format(
                        "{0} Null Exchange loaded for product:{1}. Current UTC time used instead.",
                        errorMessage,
                        product.Name ?? "Unknown");
                }
                else if (product.Exchange.TimeZoneId == null)
                {
                    errorMessage = String.Format(
                        "{0} Null TimeZoneId loaded for product:{1}. Current UTC time used instead.",
                        errorMessage,
                        product.Name ?? "Unknown");
                }
                else
                {
                    errorMessage = String.Format(
                        "{0} Please check TimeZoneId:{1} and TransactTime:{2} passed. Current UTC time used instead.",
                        errorMessage,
                        product.Exchange.TimeZoneId,
                        transactTime);
                }

                log.Error(ex, errorMessage);
            }

            return DateTime.UtcNow;
        }
    }

}