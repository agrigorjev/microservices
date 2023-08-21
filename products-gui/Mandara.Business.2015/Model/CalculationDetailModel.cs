using Mandara.Entities;
using Mandara.Entities.Calculation;
using Mandara.Entities.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mandara.Business.Client.Extensions;
using Mandara.Business.Model.Positions;
using Mandara.Date;
using Mandara.Entities.Positions;
using MoreLinq.Extensions;
using NLog;

namespace Mandara.Business.Model
{
    [Serializable]
    public class CalculationDetailModel
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private const int DailyProductIdShift = 10000;

        public Guid DetailId { get; private set; }
        public string Key { get; private set; }
        public DateTime CalculationDate { get; private set; }
        public String ProductCategory { get; private set; }
        public String ProductCategoryAbbreviation { get; private set; }
        public String Product { get; private set; }
        public String Source { get; private set; }
        public bool IsBfoe { get; private set; }
        public decimal AbsAmount { get; set; }
        public Decimal Amount { get; private set; }
        public Decimal AmountInner { get; private set; }
        public Int32 ProductId { get; private set; }
        public Int32 ProductCategoryId { get; private set; }
        public Int32 SourceProductId { get; private set; }
        public string MappingColumn { get; private set; }
        public decimal? PnlFactor { get; private set; }
        public decimal? PositionFactor { get; private set; }
        public int? PortfolioId { get; private set; }
        public CoeffEntityId EntityId { get; set; }
        public int OfficialProductId { get; private set; }
        public DateTime? CalendarDaySwapSettlementPriceDate { get; set; }
        // These are the entity position contributions.  So if the absolute total is 100 and a security position
        // contributes 50 of that then the coefficient for that entity is 0.5.
        // I.e. this should be renamed to something that conveys at least some of this information.
        public Dictionary<CoeffEntityId, decimal> Coefficients { get; set; }
        public SortedList<DateTime, DateQuantity> QuantityByDate { get; private set; }
        public ConcurrentBag<DailyDetail> DailyDetails { get; private set; }
        public string StripName { get; private set; }

        public CalculationDetailModel()
        {
            Coefficients = new Dictionary<CoeffEntityId, decimal>();
            QuantityByDate = new SortedList<DateTime, DateQuantity>();
            DailyDetails = new ConcurrentBag<DailyDetail>(new List<DailyDetail>());
        }

        public CalculationDetailModel(
            ICalculationDetailIdentifierService posIdentifier,
            Product product,
            Product sourceProduct,
            int productYear,
            int productMonth,
            decimal amountAtMonth,
            int? portfolioId,
            CoeffEntityId entityId,
            int dailyDiffMonthShift = 0,
            List<DailyDetail> dailyDetails = null,
            string stripName = null,
            DateTime? calendarDaySwapSettlementPriceDate = null)
        {
            CreatePositionCoefficient(entityId);
            QuantityByDate = new SortedList<DateTime, DateQuantity>();
            CalendarDaySwapSettlementPriceDate = calendarDaySwapSettlementPriceDate;

            DateTime calculationDate = new DateTime(productYear, productMonth, 1);
            ProductCategory category = HasOpenCategoryOverride(product, calculationDate)
                ? product.CategoryOverride
                : product.Category;
            OfficialProduct underlyingFutures = HasOpenFuturesOverride(product, calculationDate)
                ? product.UnderlyingFuturesOverride
                : product.UnderlyingFutures;

            CalculationDate = calculationDate;
            Product = product.Name;
            IsBfoe = product.IsBfoe();
            Source = sourceProduct.Name;
            ProductCategoryId = category?.CategoryId ?? product.ProductId;
            ProductCategory = category == null ? product.Name : category.Name;
            ProductCategoryAbbreviation = category == null
                ? product.Name
                : string.IsNullOrEmpty(category.Abbreviation)
                    ? category.Name
                    : category.Abbreviation;
            Amount = AmountInner = amountAtMonth;

            AbsAmount = Math.Abs(amountAtMonth);
            ProductId = product.ProductId;
            SourceProductId = GetSourceProductId(sourceProduct, dailyDiffMonthShift);
            MappingColumn = (underlyingFutures == null)
                ? product.OfficialProduct?.MappingColumn
                : underlyingFutures.MappingColumn;
            OfficialProductId = product.OfficialProduct?.OfficialProductId ?? 0;
            PnlFactor = product.PnlFactor;
            PositionFactor = product.PositionFactor;
            PortfolioId = portfolioId;
            EntityId = entityId;
            DailyDetails = new ConcurrentBag<DailyDetail>(dailyDetails ?? new List<DailyDetail>());

            DetailId = Guid.NewGuid();
            StripName = stripName;

            Key = posIdentifier.GetKey(
                ProductId,
                SourceProductId,
                StripName,
                CalculationDate,
                PortfolioId,
                calendarDaySwapSettlementPriceDate);
        }

        private void CreatePositionCoefficient(CoeffEntityId entityId)
        {
            if (null != entityId)
            {
                Coefficients = new Dictionary<CoeffEntityId, decimal>() { { entityId, 1.0M } };
            }
            else
            {
                Logger.Warn("Null entity ID received.  Position coefficients will be incorrect");
            }
        }

        private static bool HasOpenCategoryOverride(Product product, DateTime calculationDate)
        {
            return product.CategoryOverride != null
                   && product.CategoryOverrideAt != null
                   && calculationDate.FirstDayOfMonth() >= product.CategoryOverrideAt.Value.Date;
        }

        private static bool HasOpenFuturesOverride(Product product, DateTime calculationDate)
        {
            return product.CategoryOverrideAt != null
                   && product.UnderlyingFuturesOverride != null
                   && calculationDate >= product.CategoryOverrideAt.Value.Date;
        }

        public CalculationDetailModel(
            ICalculationDetailIdentifierService posIdentifier,
            Product product,
            Product sourceProduct,
            DateTime productMonth,
            DateTime positionDate,
            decimal amountOnDay,
            int? portfolioId,
            CoeffEntityId entityId,
            int dailyDiffMonthShift = 0,
            List<DailyDetail> dailyDetails = null,
            string stripName = null,
            DateTime? calendarDaySwapSettlementPriceDate = null)
        {
            CreatePositionCoefficient(entityId);
            QuantityByDate = new SortedList<DateTime, DateQuantity>();
            CalendarDaySwapSettlementPriceDate = calendarDaySwapSettlementPriceDate;

            DateTime calculationDate = productMonth.FirstDayOfMonth();
            ProductCategory category = HasOpenCategoryOverride(product, calculationDate)
                ? product.CategoryOverride
                : product.Category;
            OfficialProduct underlyingFutures = HasOpenFuturesOverride(product, calculationDate)
                ? product.UnderlyingFuturesOverride
                : product.UnderlyingFutures;

            CalculationDate = calculationDate;
            Product = product.Name;
            IsBfoe = product.IsBfoe();
            Source = sourceProduct.Name;
            ProductCategoryId = category?.CategoryId ?? product.ProductId;
            ProductCategory = category == null ? product.Name : category.Name;
            ProductCategoryAbbreviation = category == null
                ? product.Name
                : string.IsNullOrEmpty(category.Abbreviation)
                    ? category.Name
                    : category.Abbreviation;
            Amount = AmountInner = amountOnDay;

            AbsAmount = Math.Abs(amountOnDay);
            ProductId = product.ProductId;
            SourceProductId = GetSourceProductId(sourceProduct, dailyDiffMonthShift);
            MappingColumn = (underlyingFutures == null)
                ? product.OfficialProduct?.MappingColumn
                : underlyingFutures.MappingColumn;
            OfficialProductId = product.OfficialProduct?.OfficialProductId ?? 0;
            PnlFactor = product.PnlFactor;
            PositionFactor = product.PositionFactor;
            PortfolioId = portfolioId;
            EntityId = entityId;
            DailyDetails = new ConcurrentBag<DailyDetail>(dailyDetails ?? new List<DailyDetail>());

            DetailId = Guid.NewGuid();
            StripName = stripName;

            Key = posIdentifier.GetKey(
                ProductId,
                SourceProductId,
                StripName,
                CalculationDate.FirstDayOfMonth(),
                PortfolioId,
                calendarDaySwapSettlementPriceDate);

            QuantityByDate.Add(positionDate, new DateQuantity(positionDate, Amount));
        }

        public int GetSourceProductId(Product source, int dailyDiffMonthShift)
        {
            return source.ProductId + dailyDiffMonthShift * DailyProductIdShift;
        }

        public int GetOriginalSourceId(int dailyDiffMonthShift)
        {
            return SourceProductId - dailyDiffMonthShift * DailyProductIdShift;
        }

        public void Add(CalculationDetailModel newDetail, bool trackCoefficients = true)
        {
            decimal oldAbsAmount = AbsAmount;

            Amount += newDetail.Amount;
            AmountInner = Amount;
            AbsAmount += Math.Abs(newDetail.Amount);

            AddDailyDetails(newDetail.DailyDetails);

            if (trackCoefficients)
            {
                Coefficients = CoefficientsCalculator.AddCoefficients(
                    Coefficients,
                    newDetail.EntityId,
                    newDetail.Amount,
                    AbsAmount,
                    oldAbsAmount);
            }

            AddDailyQuantities(newDetail.QuantityByDate);
        }

        private void AddDailyDetails(ConcurrentBag<DailyDetail> newDailyDetails)
        {
            if (newDailyDetails == null)
            {
                return;
            }

            if (DailyDetails == null)
            {
                DailyDetails = newDailyDetails;
                return;
            }

            foreach (var newDailyDetail in newDailyDetails)
            {
                DailyDetails.Add(newDailyDetail);
            }
        }

        private void AddDailyQuantities(SortedList<DateTime, DateQuantity> quantityByDate)
        {
            quantityByDate.ForEach(
                dailyQuantity =>
                {
                    DateQuantity dayQuantity = dailyQuantity.Value;

                    if (QuantityByDate.TryGetValue(dailyQuantity.Value.PosDate, out DateQuantity quantityForDate))
                    {
                        quantityForDate.Add(dayQuantity.Quantity);
                    }
                    else
                    {
                        QuantityByDate.Add(
                            dailyQuantity.Value.PosDate,
                            new DateQuantity(dayQuantity.PosDate, dayQuantity.Quantity));
                    }
                });
        }

        public void Remove(CalculationDetailModel newDetail, bool trackCoefficients = true)
        {
            decimal oldAbsAmount = AbsAmount;

            Amount -= newDetail.Amount;
            AmountInner = Amount;

            AbsAmount -= Math.Abs(newDetail.Amount);

            RemoveDailyDetails(newDetail.DailyDetails);

            if (trackCoefficients)
            {
                Coefficients = CoefficientsCalculator.RemoveCoefficients(
                    Coefficients,
                    newDetail.EntityId,
                    newDetail.Amount,
                    AbsAmount,
                    oldAbsAmount);
            }

            RemoveDailyQuantities(newDetail);
        }

        private void RemoveDailyDetails(ConcurrentBag<DailyDetail> dailyPosToRemove)
        {
            if (dailyPosToRemove == null || dailyPosToRemove.IsEmpty || DailyDetails == null || DailyDetails.IsEmpty)
            {
                return;
            }

            DailyDetails = new ConcurrentBag<DailyDetail>(DailyDetails.Except(dailyPosToRemove));
        }

        private void RemoveDailyQuantities(CalculationDetailModel newDetail)
        {
            newDetail.QuantityByDate.Values.ForEach(
                dayQty =>
                {
                    if (!QuantityByDate.ContainsKey(dayQty.PosDate))
                    {
                        Logger.Debug("Adding key for {0} with date {1}", newDetail.Key, dayQty.PosDate);
                        QuantityByDate.Add(dayQty.PosDate,new DateQuantity(dayQty.PosDate, 0));
                    }
                    
                    DateQuantity currentDayPos = SubtractDayQuantity(dayQty);

                    if (0M == currentDayPos.Quantity && !currentDayPos.PosDate.IsWeekendDay())
                    {
                        Logger.Debug("Removing key for {0} with date {1}", newDetail.Key, dayQty.PosDate);
                        QuantityByDate.Remove(dayQty.PosDate);
                    }
                });

            DateQuantity SubtractDayQuantity(DateQuantity dayQty)
            {
                DateQuantity currentDayPos = QuantityByDate[dayQty.PosDate];

                currentDayPos.Subtract(dayQty.Quantity);
                return currentDayPos;
            }
        }

        public override string ToString()
        {
            StringBuilder textValue = new StringBuilder();

            textValue.AppendFormat("Key = {0}; ", Key);
            textValue.AppendFormat("Product = {0}; ", Product);
            textValue.AppendFormat("Product ID = {0}; ", ProductId);
            textValue.AppendFormat("Source Product ID = {0}; ", SourceProductId);
            textValue.AppendFormat("Amount = {0:F2}; ", AmountInner);
            textValue.AppendFormat("Date = {0}; ", CalculationDate);
            textValue.AppendFormat("QuantityByDate = ({0})", Format(QuantityByDate));

            return textValue.ToString();
        }

        private static string Format(SortedList<DateTime, DateQuantity> quantityByDate)
        {
            if (quantityByDate.Count == 1)
            {
                var tupple = quantityByDate.First();
                return $"{tupple.Key:d}:{tupple.Value.Quantity:F2}";
            }
            return string.Join(";", quantityByDate.Select(x => $"{x.Key:d}:{x.Value.Quantity:F2}"));
        }

        public static CalculationDetailModel Aggregate(IEnumerable<CalculationDetailModel> daily)
        {
            var basePos = daily.First();
            var totalAmount = daily.Sum(pos => pos.Amount);

            CalculationDetailModel copy = new CalculationDetailModel()
            {
                DetailId = Guid.NewGuid(),
                CalculationDate = basePos.CalculationDate,
                ProductCategory = basePos.ProductCategory,
                ProductCategoryAbbreviation = basePos.ProductCategoryAbbreviation,
                Product = basePos.Product,
                Source = basePos.Source,
                IsBfoe = basePos.IsBfoe,
                AbsAmount = Math.Abs(totalAmount),
                Amount = totalAmount,
                AmountInner = totalAmount,
                ProductId = basePos.ProductId,
                ProductCategoryId = basePos.ProductCategoryId,
                SourceProductId = basePos.SourceProductId,
                MappingColumn = basePos.MappingColumn,
                PnlFactor = basePos.PnlFactor,
                PositionFactor = basePos.PositionFactor,
                PortfolioId = basePos.PortfolioId,
                EntityId = basePos.EntityId,
                OfficialProductId = basePos.OfficialProductId,
                CalendarDaySwapSettlementPriceDate = basePos.CalendarDaySwapSettlementPriceDate,
                Coefficients = basePos.Coefficients,
                QuantityByDate = new SortedList<DateTime, DateQuantity>(basePos.QuantityByDate),
                DailyDetails = new ConcurrentBag<DailyDetail>(daily.SelectMany(dayPos => dayPos.DailyDetails)),
                StripName = basePos.StripName,
                Key = basePos.Key,
            };

            return copy;
        }

        public override bool Equals(object obj)
        {
            if (obj is CalculationDetailModel rhsPos)
            {
                return Object.ReferenceEquals(this, obj) || Equals(rhsPos);
            }

            return false;
        }

        public bool Equals(CalculationDetailModel rhs)
        {
            return CalculationDate == rhs.CalculationDate
                   && ProductCategory == rhs.ProductCategory
                   && ProductCategoryAbbreviation == rhs.ProductCategoryAbbreviation
                   && Product == rhs.Product
                   && Source == rhs.Source
                   && IsBfoe == rhs.IsBfoe
                   && Math.Abs((int)(1000 * AbsAmount) - (int)(1000 * rhs.AbsAmount)) <= 1
                   && Math.Abs((int)(1000 * Amount) - (int)(1000 * rhs.Amount)) <= 1
                   && Math.Abs((int)(1000 * AmountInner) - (int)(1000 * rhs.AmountInner)) <= 1
                   && ProductId == rhs.ProductId
                   && ProductCategoryId == rhs.ProductCategoryId
                   && SourceProductId == rhs.SourceProductId
                   && MappingColumn == rhs.MappingColumn
                   && PnlFactor == rhs.PnlFactor
                   && PositionFactor == rhs.PositionFactor
                   && PortfolioId == rhs.PortfolioId
                   && EntityId.Equals(rhs.EntityId)
                   && OfficialProductId == rhs.OfficialProductId
                   && CalendarDaySwapSettlementPriceDate == rhs.CalendarDaySwapSettlementPriceDate
                   && EqualCoefficients(rhs.Coefficients)
                   && EqualDatedQuantities(rhs.QuantityByDate)
                   && EqualDailyDetails(rhs.DailyDetails)
                   && StripName == rhs.StripName
                   && Key == rhs.Key;
        }

        private bool EqualCoefficients(Dictionary<CoeffEntityId, decimal> rhs)
        {
            return Coefficients.Keys.All(
                lhsId => rhs.TryGetValue(lhsId, out decimal rhsCoeff)
                         && rhsCoeff == lhsId.EntityId);
        }

        private bool EqualDatedQuantities(SortedList<DateTime, DateQuantity> rhs)
        {
            return QuantityByDate.Keys.All(lhsDate =>
                rhs.TryGetValue(lhsDate, out DateQuantity rhsQuantity)
                && rhsQuantity.Quantity == QuantityByDate[lhsDate].Quantity);
        }

        private bool EqualDailyDetails(ConcurrentBag<DailyDetail> rhs)
        {
            List<DailyDetail> lhsDailies = DailyDetails
                                           .ToArray()
                                           .OrderBy(lhsDaily => lhsDaily.CalculationDate)
                                           .ThenBy(lhsDaily => lhsDaily.Amount)
                                           .ToList();
            List<DailyDetail> rhsDailies = rhs.ToArray()
                                              .OrderBy(rhsDaily => rhsDaily.CalculationDate)
                                              .ThenBy(rhsDaily => rhsDaily.Amount)
                                              .ToList();
            bool allEqual = Enumerable.Range(0, lhsDailies.Count)
                                      .All(index => lhsDailies[index].Equals(rhsDailies[index]));

            return DailyDetails.Count == rhs.Count && allEqual;
        }

        public static readonly DateTime NoDate = DateTime.MinValue;
        public const decimal NoAmount = decimal.MinValue;
        public const int NoId = -1;
        public const string NoName = "";
        public const decimal NoFactor = 0M;

        public static readonly CalculationDetailModel Default = new CalculationDetailModel()
        {
            CalculationDate = NoDate,
            ProductCategory = NoName,
            ProductCategoryAbbreviation = NoName,
            Product = NoName,
            Source = NoName,
            IsBfoe = false,
            AbsAmount = NoAmount,
            Amount = NoAmount,
            AmountInner = NoAmount,
            ProductId = NoId,
            ProductCategoryId = NoId,
            SourceProductId = NoId,
            MappingColumn = NoName,
            PnlFactor = NoFactor,
            PositionFactor = NoFactor,
            PortfolioId = NoId,
            EntityId = CoeffEntityId.Default,
            OfficialProductId = NoId,
            CalendarDaySwapSettlementPriceDate = NoDate,
            Coefficients = new Dictionary<CoeffEntityId, decimal>(),
            QuantityByDate = new SortedList<DateTime, DateQuantity>(),
            DailyDetails = new ConcurrentBag<DailyDetail>(),
        };

        public bool IsDefault() => Default.Equals(this);
    }
}
