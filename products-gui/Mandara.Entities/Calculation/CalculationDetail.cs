using AutoMapper;
using Mandara.Entities.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Mandara.Entities.Positions;

namespace Mandara.Entities.Calculation
{
    public class CalculationDetail
    {
        public Guid DetailId { get; set; }

        public DateTime CalculationDate { get; set; }
        public String ProductCategory { get; set; }
        public String ProductCategoryAbbreviation { get; set; }
        public String Product { get; set; }
        public Product ProductReference { get; set; }
        public String Source { get; set; }
        public Decimal Amount { get; set; }
        public Int32 ProductId { get; set; }
        public Int32 ProductCategoryId { get; set; }
        public Int32 SourceProductId { get; set; }
        public DateTime ProductDate { get; set; }

        public ProductDateType ProductDateType { get; set; }
        public string MappingColumn { get; set; }
        public decimal? PnlFactor { get; set; }
        public decimal? PositionFactor { get; set; }

        public decimal HistoricalAmount { get; set; }
        public String DataType { get; set; }

        public List<SourceDetail> SourceDetails { get; set; }
        public List<KeyValuePair<int, decimal>> SourceDetailAmounts { get; set; }
        public ConcurrentDictionary<int, decimal> SourceDetailAmountsDict { get; set; }

        public Decimal AmountInner { get; set; }

        public int? PortfolioId { get; set; }

        public List<Guid> CalculationDetailsIds { get; set; }

        public List<CalculationDetail> OriginalPositions { get; set; }

        public int OfficialProductId { get; set; }

        public SortedList<DateTime, DateQuantity> QuantityByDate { get; set; } =
            new SortedList<DateTime, DateQuantity>();
        public ConcurrentBag<DailyDetail> DailyDetails { get; set; }

        public string StripName { get; set; }

        private static readonly CalculationDetailIdentifierWithoutStripNameService _calculationDetailIdentifierService
            = new CalculationDetailIdentifierWithoutStripNameService();

        public string GetKey()
        {
            return _calculationDetailIdentifierService.GetKey(
                ProductId,
                SourceProductId,
                StripName,
                CalculationDate,
                PortfolioId,
                null);
        }

        public static CalculationDetail Create(
            SourceDetail sourceDetail,
            Product product,
            Product sourceProduct,
            int productYear,
            int productMonth,
            decimal amountAtMonth,
            DateTime productDate,
            ProductDateType dateType,
            string stripName,
            string productNameOverride,
            string sourceNameOverride,
            int dailyDiffMonthShift)
        {
            DateTime calculationDate = new DateTime(productYear, productMonth, 1);
            ProductCategory category = product.Category;

            if (product.CategoryOverride != null && product.CategoryOverrideAt != null)
            {
                if (calculationDate >= product.CategoryOverrideAt.Value.Date)
                {
                    category = product.CategoryOverride;
                }
            }

            OfficialProduct underlyingFutures = product.UnderlyingFutures;

            if (product.CategoryOverrideAt != null && product.UnderlyingFuturesOverride != null)
            {
                if (calculationDate >= product.CategoryOverrideAt.Value.Date)
                    underlyingFutures = product.UnderlyingFuturesOverride;
            }

            CalculationDetail calculation = new CalculationDetail
            {
                DetailId = Guid.NewGuid(),
                CalculationDate = calculationDate,
                Product = productNameOverride ?? product.Name,
                Source = sourceNameOverride ?? sourceProduct.Name,
                ProductCategoryId = category == null ? product.ProductId : category.CategoryId,
                ProductCategory = category == null ? product.Name : category.Name,
                ProductCategoryAbbreviation = category == null
                        ? product.Name
                        : string.IsNullOrEmpty(category.Abbreviation)
                             ? category.Name
                             : category.Abbreviation,
                Amount = amountAtMonth,
                ProductReference = product,
                ProductId = product.ProductId + dailyDiffMonthShift * 10000,
                SourceProductId = sourceProduct.ProductId + dailyDiffMonthShift * 10000,
                ProductDate = productDate,
                ProductDateType = dateType,
                MappingColumn =
                    (underlyingFutures == null)
                        ? product.OfficialProduct?.MappingColumn
                        : underlyingFutures.MappingColumn,
                OfficialProductId = product.OfficialProduct?.OfficialProductId ?? 0,
                PnlFactor = product.PnlFactor,
                PositionFactor = product.PositionFactor,
                StripName = stripName,
            };

            calculation.QuantityByDate.Add(calculationDate, new DateQuantity(calculationDate, amountAtMonth));
            AddSourceDetailToCalculationDetail(calculation, sourceDetail, amountAtMonth);

            return calculation;
        }

        private static void AddSourceDetailToCalculationDetail(
            CalculationDetail calculation,
            SourceDetail sourceDetail,
            decimal amountAtMonth)
        {
            if (sourceDetail.SourceDetailId > 0)
            {
                if (calculation.SourceDetails == null)
                {
                    calculation.SourceDetails = new List<SourceDetail>();
                }

                if (calculation.SourceDetailAmountsDict == null)
                {
                    calculation.SourceDetailAmountsDict = new ConcurrentDictionary<int, decimal>();
                }

                if (calculation.SourceDetailAmountsDict.ContainsKey(sourceDetail.SourceDetailId))
                {
                    int key = sourceDetail.SourceDetailId;

                    if (calculation.SourceDetailAmountsDict.TryGetValue(key, out decimal sdAmount))
                    {
                        calculation.SourceDetailAmountsDict.TryRemove(key, out decimal _);
                        calculation.SourceDetailAmountsDict.TryAdd(key, sdAmount + amountAtMonth);
                    }
                }
                else
                {
                    calculation.SourceDetails.Add(sourceDetail);
                    calculation.SourceDetailAmountsDict.TryAdd(sourceDetail.SourceDetailId, amountAtMonth);
                }
            }
        }

        public void FillDailyDetails(List<DateTime> businessDays, int businessDaysElapsed)
        {
            if (businessDaysElapsed == -1)
            {
                businessDaysElapsed = 0;
            }

            if (businessDays.Count - businessDaysElapsed <= 0)
            {
                return;}

            decimal dailyAmount = Amount / (businessDays.Count - businessDaysElapsed);

            DailyDetails = new ConcurrentBag<DailyDetail>();

            for (int i = businessDaysElapsed; i < businessDays.Count; i++)
            {
                DailyDetails.Add(new DailyDetail(businessDays[i], dailyAmount));
            }
        }

        public static SortedList<DateTime, DateQuantity> ToQuantityByDate(IEnumerable<DateQuantity> datedQuantities)
        {
            return datedQuantities.Aggregate(
                new SortedList<DateTime, DateQuantity>(),
                (quantityByDate, quantity) =>
                {
                    quantityByDate.Add(quantity.PosDate, quantity);
                    return quantityByDate;
                });
        }

        public List<CalculationDetail> SplitDaily()
        {
            return QuantityByDate.Select(
                dailyPos => new CalculationDetail()
                {
                    DetailId = Guid.NewGuid(),
                    CalculationDate = dailyPos.Key,
                    ProductCategory = this.ProductCategory,
                    ProductCategoryAbbreviation = this.ProductCategoryAbbreviation,
                    Product = this.Product,
                    ProductReference = this.ProductReference,
                    Source = this.Source,
                    Amount = dailyPos.Value.Quantity,
                    AmountInner = dailyPos.Value.Quantity,
                    ProductId = this.ProductId,
                    ProductCategoryId = this.ProductCategoryId,
                    SourceProductId = this.SourceProductId,
                    // ProductDate isn't set.  That problem should be fixed so that ProductDate comes from the Product
                    ProductDate = this.CalculationDate,
                    ProductDateType = this.ProductDateType,
                    MappingColumn = this.MappingColumn,
                    PnlFactor = this.PnlFactor,
                    PositionFactor = this.PositionFactor,
                    HistoricalAmount = this.HistoricalAmount,
                    DataType = this.DataType,
                    SourceDetails = this.SourceDetails,
                    SourceDetailAmounts = this.SourceDetailAmounts,
                    SourceDetailAmountsDict = this.SourceDetailAmountsDict,
                    PortfolioId = this.PortfolioId,
                    CalculationDetailsIds = this.CalculationDetailsIds,
                    OriginalPositions = this.OriginalPositions,
                    OfficialProductId = this.OfficialProductId,
                    DailyDetails = this.DailyDetails,
                    StripName = this.StripName,
                }).ToList();
        }
    }
}
