using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.EntitiesCustomization;
using Mandara.Entities.ErrorReporting;
using System;
using System.Collections.Generic;
using System.Data;
using Mandara.Business.Calculators;
using Mandara.Date;

namespace Mandara.Business.OldCode
{
    public class SnapshotPriceGetter : IPriceGetter
    {
        private readonly DataTable _priceData;
        private readonly DateTime? _snapshotDatetime;
        private static readonly Dictionary<string, DateTime> _errors = new Dictionary<string, DateTime>();

        public SnapshotPriceGetter(DataTable priceData, DateTime? snapshotDatetime)
        {
            _priceData = priceData;
            _snapshotDatetime = snapshotDatetime;
        }

        public decimal? GetTradePrice(TradeCapture trade)
        {
            if (!_snapshotDatetime.HasValue || trade?.SecurityDefinition?.StripName == null)
            {
                return null;
            }

            List<decimal> overnightPrice = new List<decimal>();

            foreach (StripPart stripPart in new[] { trade.Strip.Part1, trade.Strip.Part2 })
            {
                if (stripPart.IsDefault())
                {
                    continue;
                }

                Product product = trade.SecurityDefinition.Product;
                
                if (product == null)
                {
                    ErrorReportingHelper.GlobalQueue.Enqueue(
                        new Error(
                            "Snapshot Price",
                            ErrorType.TradeError,
                            "Product reference not set.",
                            null,
                            trade,
                            ErrorLevel.Normal));

                    return null;
                }

                OfficialProduct officialProduct = product.OfficialProduct;
                
                if (officialProduct == null)
                {
                    ErrorReportingHelper.GlobalQueue.Enqueue(
                        new Error(
                            "Snapshot Price",
                            ErrorType.TradeError,
                            "Official product reference not set.",
                            null,
                            trade,
                            ErrorLevel.Normal));

                    return null;
                }

                decimal? price = GetProductPrice(
                    product.ProductId,
                    stripPart.StartDate,
                    stripPart.DateType,
                    officialProduct.MappingColumn,
                    trade.TradeStartDate,
                    trade.TradeEndDate);

                overnightPrice.Add(price ?? 0M);
            }

            switch (overnightPrice.Count) 
            {
                case 2:
                {
                    return overnightPrice[0] - overnightPrice[1];
                }

                case 1:
                {
                    return overnightPrice[0];
                }

                default:
                {
                    return null;
                }
            }
        }

        public decimal? GetProductPrice(
            int productId,
            DateTime productDate,
            ProductDateType productDateType,
            string mappingColumn,
            object tradeStartDateObject = null,
            object tradeEndDateObject = null)
        {
            if (!_snapshotDatetime.HasValue || string.IsNullOrEmpty(mappingColumn))
            {
                return null;
            }

            DateTime riskDate = _snapshotDatetime.Value.Date;

            try
            {
                switch (productDateType)
                {
                    case ProductDateType.Day:
                    case ProductDateType.Daily:
                    case ProductDateType.MonthYear:
                    {
                        return DayOrMonthPrice(_snapshotDatetime.Value, productDate, riskDate, mappingColumn);
                    }

                    case ProductDateType.Quarter:
                    {
                        return MultiMonthStripPrice(productDate, mappingColumn, riskDate, 2);
                    }

                    case ProductDateType.Year:
                    {
                        return MultiMonthStripPrice(productDate, mappingColumn, riskDate, 11);
                    }

                    case ProductDateType.Custom:
                    {
                        return CustomStripPrice(mappingColumn, tradeStartDateObject, tradeEndDateObject);
                    }
                }
            }
            catch (Exception whatWentWrong)
            {
                OnProductPriceError(mappingColumn, whatWentWrong);
            }

            return 0M;
        }

        private decimal DayOrMonthPrice(
            DateTime snapshotDate,
            DateTime productDate,
            DateTime riskDate,
            string priceColumn)
        {
            Int32 productMonth = PriceMonth.Get(riskDate, productDate);

            if (productMonth < 0)
            {
                return Convert.ToDecimal(0M);
            }

            double totalSeconds = EpochConverter.ToEpochTime(snapshotDate);
            DataRow priceRow = _priceData.Rows.Find(new object[] { totalSeconds, productMonth });

            return
                null == priceRow?.Table || !priceRow.Table.Columns.Contains(priceColumn)
                    ? 0M
                    : null == priceColumn
                        ? 0M
                        : priceRow.IsNull(priceColumn)
                            ? 0M
                            : Convert.ToDecimal(priceRow.Field<Double>(priceColumn));
        }

        /// <summary>
        /// Note the assumption that the start date is valid for the multi-month type - quarter or cal.  There is no
        /// attempt to check that the product date is the start of a quarter, or that the risk date is in that quarter,
        /// for instance.
        /// </summary>
        /// <param name="productDate"></param>
        /// <param name="priceColumn"></param>
        /// <param name="riskDate"></param>
        /// <param name="additionalMonths"></param>
        /// <returns></returns>
        private decimal MultiMonthStripPrice(
            DateTime productDate,
            string priceColumn,
            DateTime riskDate,
            int additionalMonths)
        {
            string expression = AveragePriceColumn(priceColumn);
            string filter = MonthFilter(PriceMonth.Get(riskDate, productDate), additionalMonths);
            Object price = _priceData.Compute(expression, filter);

            return price is DBNull ? 0M : Convert.ToDecimal((Double)price);
        }

        private decimal CustomStripPrice(
            string mappingColumn,
            object tradeStartDateObject,
            object tradeEndDateObject)
        {
            DateTime tradeStartDate = StripHelper.ParseDate(tradeStartDateObject, Formats.SortableShortDate)
                                      ?? DateTime.MinValue;
            DateTime tradeEndDate = StripHelper.ParseDate(tradeEndDateObject, Formats.SortableShortDate) ?? DateTime.MinValue;

            if (DateTime.MinValue != tradeStartDate && DateTime.MinValue != tradeEndDate)
            {
                int numAddOnMonths = tradeEndDate.MonthsSince(tradeStartDate);
                string expression = AveragePriceColumn(mappingColumn);
                string filter = MonthFilter(PriceMonth.Get(tradeStartDate, SystemTime.Now()), numAddOnMonths);
                object customPriceObj = _priceData.Compute(expression, filter);

                return customPriceObj is DBNull ? 0M : Convert.ToDecimal((Double)customPriceObj);
            }

            return 0M;
        }

        private static void OnProductPriceError(string mappingColumn, Exception ex)
        {
            bool reportError = false;

            if (_errors.TryGetValue(mappingColumn, out DateTime errorTime))
            {
                if (SystemTime.Now() - errorTime > TimeSpan.FromMinutes(1))
                {
                    reportError = true;

                    _errors.Remove(mappingColumn);
                    _errors.Add(mappingColumn, SystemTime.Now());
                }
            }
            else
            {
                reportError = true;
                _errors.Add(mappingColumn, SystemTime.Now());
            }

            if (reportError)
            {
                ErrorReportingHelper.GlobalQueue.Enqueue(
                    new Error(
                        "Snapshot Price",
                        ErrorType.Exception,
                        "There was an error while getting snapshot price for a product.",
                        null,
                        ex,
                        ErrorLevel.Critical));
            }
        }

        private static string AveragePriceColumn(string priceColumn)
        {
            return $"AVG({priceColumn})";
        }

        private static string MonthFilter(int baseMonthOffset, int offsetToEndMonth)
        {
            return $"rdate >= {baseMonthOffset} AND rdate <= {baseMonthOffset + offsetToEndMonth}";
        }
    }
}
