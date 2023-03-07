using JetBrains.Annotations;
using Mandara.Business.Contracts;
using Mandara.Business.OldCode.Calculators;
using Mandara.Entities;
using Mandara.Entities.Calculation;
using Mandara.Entities.ErrorReporting;
using Mandara.Entities.Exceptions;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Mandara.Date;

//[assembly: InternalsVisibleTo("Mandara.Business.2015")]
namespace Mandara.Business.OldCode
{
    internal interface IPositionCalculator
    {
        List<CalculationDetail> Calculate(SourceDetail sourceDetail, CalculationContext calculationContext);
    }

    internal abstract class PositionCalculatorBase : IPositionCalculator
    {
        protected CalculationManager CalculationManager { get; private set; }

        protected PositionCalculatorBase(CalculationManager calculationManager)
        {
            CalculationManager = calculationManager;
        }

        public abstract List<CalculationDetail> Calculate(
            SourceDetail sourceDetail,
            CalculationContext calculationContext);

        public virtual List<CalculationDetail> CalculateFirstLeg(SourceDetail trade, CalculationContext context)
        {
            return Calculate(trade, context);
        }

        public virtual List<CalculationDetail> CalculateSecondLeg(SourceDetail trade, CalculationContext context)
        {
            return Calculate(trade, context);
        }
    }

    internal class CalculationContext
    {
        private int _calcYear;
        private int _calcMonth;
        private int _calcDay;

        public CalculationCache CalculationCache { get; set; }

        public DateTime RiskDate { get; set; }
        public int ProductYear { get; set; }
        public int ProductMonth { get; set; }
        public int ProductDay { get; set; }

        public int CalcYear
        {
            get
            {
                if (_calcYear == 0)
                {
                    return ProductYear;
                }

                return _calcYear;
            }
            set
            {
                _calcYear = value;
            }
        }

        public int CalcMonth
        {
            get
            {
                if (_calcMonth == 0)
                {
                    return ProductMonth;
                }

                return _calcMonth;
            }
            set
            {
                _calcMonth = value;
            }
        }

        public int CalcDay
        {
            get
            {
                if (_calcDay == 0)
                {
                    return ProductDay;
                }

                return _calcDay;
            }
            set
            {
                _calcDay = value;
            }
        }

        public Product Product { get; set; }
        public Product SourceProduct { get; set; }
        public Product BalmoOnCrudeProduct { get; set; }

        public bool SuppressRolloff { get; set; }
        public bool SuppressContractSize { get; set; }
        public bool SuppressPositionConversionFactor { get; set; }

        public decimal Quantity { get; set; }
        public decimal? CrudeSwapFactor { get; set; }

        public ProductDateType ProductDateType { get; set; }
        public List<DateTime> MergedHolidayDays { get; set; }
        public int? BalmoCorrection { get; set; }

        public int DailyDiffMonthShift { get; set; }
        public bool HasDailyDiffMonthShift => DailyDiffMonthShift > 0;

        public CalculationContext GetNew(
            int? year,
            int? month,
            int? day,
            decimal? crudeSwapFactor,
            bool? suppressRolloff,
            List<DateTime> mergedHolidayDays,
            Product product,
            Product sourceProduct,
            decimal? quantity,
            bool? suppressPositionConversionFactor,
            bool? suppressContractSize,
            int? balmoCorrection,
            int dailyDiffMonthShift,
            Product balmoOnCrudeProduct,
            int? calcYear,
            int? calcMonth,
            int? calcDay)
        {
            return new CalculationContext
            {
                CalculationCache = CalculationCache,
                RiskDate = RiskDate,
                ProductYear = year ?? ProductYear,
                ProductMonth = month ?? ProductMonth,
                ProductDay = day ?? ProductDay,
                ProductDateType = ProductDateType,
                Quantity = quantity ?? Quantity,
                Product = product ?? Product,
                SourceProduct = sourceProduct ?? SourceProduct,
                CrudeSwapFactor = crudeSwapFactor ?? CrudeSwapFactor,
                SuppressContractSize = suppressContractSize ?? SuppressContractSize,
                SuppressPositionConversionFactor = suppressPositionConversionFactor ?? SuppressPositionConversionFactor,
                SuppressRolloff = suppressRolloff ?? SuppressRolloff,
                MergedHolidayDays = mergedHolidayDays ?? MergedHolidayDays,
                BalmoCorrection = balmoCorrection ?? BalmoCorrection,
                DailyDiffMonthShift = GetDailyDiffMonthShift(dailyDiffMonthShift),
                BalmoOnCrudeProduct = balmoOnCrudeProduct ?? BalmoOnCrudeProduct,
                CalcYear = calcYear ?? year ?? CalcYear,
                CalcMonth = calcMonth ?? month ?? CalcMonth,
                CalcDay = calcDay ?? day ?? CalcDay
            };
        }

        private static int GetDailyDiffMonthShift(int dailyDiffMonthShift)
        {
            return dailyDiffMonthShift > Product.NoDailyDiffMonthShift
                ? dailyDiffMonthShift
                : Product.NoDailyDiffMonthShift;
        }
    }

    public class ClassicPositionsCalculator
    {
        private CalculationManager _calculationManager;
        private CalculationCache _calculationCache;
        private readonly MandaraEntities _efContext;

        private SwapPositionCalculator _swapPositionCalculator;
        private TradeMonthSwapPositionCalculator _tradeMonthSwapPositionCalculator;
        private FuturesPositionCalculator _futuresPositionCalculator;
        private CrudeSwapPositionCalculator _crudeSwapPositionCalculator;
        private CrackSpreadDiffPositionCalculator _crackSpreadDiffPositionCalculator;
        private BalmoPositionCalculator _balmoPositionCalculator;
        private DailySwapPositionCalculator _dailySwapPositionCalculator;
        private DailyDiffPositionCalculator _dailyDiffPositionCalculator;
        private readonly ILogger _logger = new NLogLoggerFactory().GetCurrentClassLogger();

        private readonly Dictionary<bool, Func<string, object, Action<SourceDetail>>> _reportErrorActionGenerators =
            new Dictionary<bool, Func<string, object, Action<SourceDetail>>>()
            {
                { true, ReportCalculationError },
                { false, DoNothingWithSourceDetail },
            };

        public ClassicPositionsCalculator([NotNull] MandaraEntities efContext, CalculationCache calculationCache = null)
        {
            if (efContext == null)
            {
                throw new ArgumentNullException("efContext", "efContext should not be null");
            }

            _calculationCache = calculationCache;
            _efContext = efContext;

            Initialize();
        }

        private void Initialize()
        {
            if (_calculationCache == null)
            {
                _calculationCache = new CalculationCache();
                _calculationCache.Initialize();
            }

            _calculationManager = new CalculationManager();

            CreatePositionCalculators();
        }

        private void CreatePositionCalculators()
        {
            _swapPositionCalculator = new SwapPositionCalculator(_calculationManager);
            _tradeMonthSwapPositionCalculator = new TradeMonthSwapPositionCalculator(
                _calculationManager,
                IoC.Get<IProductsStorage>());

            _futuresPositionCalculator = new FuturesPositionCalculator(_calculationManager, _swapPositionCalculator);

            _crudeSwapPositionCalculator = new CrudeSwapPositionCalculator(_calculationManager, _swapPositionCalculator);

            _crackSpreadDiffPositionCalculator = new CrackSpreadDiffPositionCalculator(
                _calculationManager,
                _efContext,
                _futuresPositionCalculator,
                _crudeSwapPositionCalculator,
                _swapPositionCalculator,
                _tradeMonthSwapPositionCalculator);

            _balmoPositionCalculator = new BalmoPositionCalculator(_calculationManager, _crudeSwapPositionCalculator);

            _crackSpreadDiffPositionCalculator.BalmoPositionCalculator = _balmoPositionCalculator;
            _balmoPositionCalculator.CrackSpreadDiffPositionCalculator = _crackSpreadDiffPositionCalculator;

            _dailySwapPositionCalculator = new DailySwapPositionCalculator(_calculationManager);
            _dailyDiffPositionCalculator = new DailyDiffPositionCalculator(
                _calculationManager,
                _efContext,
                _dailySwapPositionCalculator);
        }

        private IPositionCalculator GetPositionCalculator(SourceDetail sourceDetail)
        {
            switch (sourceDetail.Product.Type)
            {
                case ProductType.Swap:
                {
                    return _swapPositionCalculator;
                }

                case ProductType.TradeMonthSwap:
                {
                    return _tradeMonthSwapPositionCalculator;
                }

                case ProductType.Futures:
                {
                    return _futuresPositionCalculator;
                }

                case ProductType.FuturesBasedSwap:
                {
                    return _crudeSwapPositionCalculator;
                }

                case ProductType.Diff:
                {
                    return _crackSpreadDiffPositionCalculator;
                }

                case ProductType.Balmo:
                {
                    return _balmoPositionCalculator;
                }

                case ProductType.DailySwap:
                {
                    return _dailySwapPositionCalculator;
                }
                case ProductType.DayVsMonthFullWeek:
                case ProductType.DayVsMonthCustom:
                case ProductType.DailyVsDaily:
                {
                    return _dailyDiffPositionCalculator;
                }

                default:
                {
                    throw new Exception("No such position calculator");
                }
            }
        }

        public List<CalculationDetail> Calculate(List<SourceDetail> sourceDetails, DateTime riskDate, bool reportErrors)
        {
            List<CalculationDetail> result = new List<CalculationDetail>();
            Dictionary<int, bool> timeSpreadsCalculated = new Dictionary<int, bool>();

            for (int index = 0; index <= sourceDetails.Count; index++)
            {
                try
                {
                    if ((index > 0) && sourceDetails[index - 1].IsTimeSpread)
                    {
                        SourceDetail timeSpread = sourceDetails[index - 1];

                        if (!timeSpreadsCalculated.ContainsKey(timeSpread.SourceDetailId))
                        {
                            index = index - 1;

                            timeSpreadsCalculated.Add(timeSpread.SourceDetailId, true);
                        }
                    }

                    if (index == sourceDetails.Count)
                    {
                        break;
                    }

                    if (sourceDetails[index].CantCalculatePositions)
                    {
                        continue;
                    }

                    SourceDetail sourceDetail = sourceDetails[index];
                    bool useSecondStrip;

                    if (!timeSpreadsCalculated.TryGetValue(sourceDetail.SourceDetailId, out useSecondStrip))
                    {
                        useSecondStrip = false;
                    }

                    DateTime productDate = !useSecondStrip ? sourceDetail.ProductDate : sourceDetail.ProductDate2;
                    ProductDateType productDateType = !useSecondStrip ? sourceDetail.DateType : sourceDetail.DateType2;
                    decimal? quantity = !useSecondStrip ? sourceDetail.Quantity : sourceDetail.Quantity * -1M;

                    riskDate = CalculationManager.AdjustRiskDateForWeekends(riskDate);
                    SetSourceDetailLegToUse(sourceDetail, useSecondStrip);

                    int productId = GetProductId(sourceDetail);
                    Product sourceProduct = GetSourceProduct(sourceDetail, productId);

                    if (DoesProductDateIncludeRiskDate(riskDate, productDate, productDateType, sourceProduct))
                    {
                        continue;
                    }

                    CalculationContext calculationContext = new CalculationContext
                    {
                        CalculationCache = _calculationCache,
                        RiskDate = riskDate,
                        ProductYear = productDate.Year,
                        ProductMonth = productDate.Month,
                        ProductDay = productDate.Day,
                        ProductDateType = productDateType,
                        Quantity = quantity.Value,
                        Product = sourceProduct,
                        SourceProduct = sourceProduct
                    };

                    result.AddRange(
                        CalculatePositions(
                            riskDate,
                            sourceDetail,
                            productDateType,
                            calculationContext,
                            productDate,
                            sourceProduct));
                }
                catch (CalendarNotFoundException ex)
                {
                    Action<SourceDetail> reportError = _reportErrorActionGenerators[reportErrors](ex.Message, null);

                    HandlePrecalcDetailCalculationError(
                        sourceDetails[index],
                        "The expiry or holiday calendar was not found",
                        reportError,
                        ex);
                }
                catch (CalendarExpiryDateNotFoundException ex)
                {
                    Action<SourceDetail> reportError = _reportErrorActionGenerators[reportErrors](ex.Message, null);

                    HandlePrecalcDetailCalculationError(
                        sourceDetails[index],
                        "The expiry calendar did not have an entry for the date selected",
                        reportError,
                        ex);
                }
                catch (Exception ex)
                {
                    Action<SourceDetail> reportError =
                        _reportErrorActionGenerators[reportErrors](
                            "Unhandled exception during positions calculation",
                            ex);

                    HandlePrecalcDetailCalculationError(sourceDetails[index], "General exception", reportError, ex);
                }
            }

            return Reduce(result);
        }

        private static int GetProductId(SourceDetail sourceDetail)
        {
            int productId = sourceDetail.ProductId;

            if ((productId == 0) && (sourceDetail.Product != null))
            {
                productId = sourceDetail.Product.ProductId;
            }
            return productId;
        }

        // TODO: Is this the correct name?
        private static bool DoesProductDateIncludeRiskDate(
            DateTime riskDate,
            DateTime productDate,
            ProductDateType productDateType,
            Product sourceProduct)
        {
            // TODO: Why are only Futures excluded from this?
            if (sourceProduct.Type == ProductType.Futures)
            {
                return false;
            }

            bool isProductMonthInBeforeRiskMonth = (productDate.Month < riskDate.Month)
                                                   && (productDate.Year <= riskDate.Year);
            bool isProductYearBeforeRiskYear = productDate.Year < riskDate.Year;
            bool couldProductPeriodIncludeMultipleMonths = (productDateType != ProductDateType.Quarter)
                                                           && (productDateType != ProductDateType.Year)
                                                           && (productDateType != ProductDateType.Custom);

            return (isProductMonthInBeforeRiskMonth || isProductYearBeforeRiskYear)
                   && couldProductPeriodIncludeMultipleMonths;
        }

        private List<CalculationDetail> CalculatePositions(
            DateTime riskDate,
            SourceDetail sourceDetail,
            ProductDateType productDateType,
            CalculationContext calculationContext,
            DateTime productDate,
            Product sourceProduct)
        {
            IPositionCalculator positionCalculator = GetPositionCalculator(sourceDetail);
            List<CalculationDetail> positions = new List<CalculationDetail>();

            switch (productDateType)
            {
                case ProductDateType.MonthYear:
                {
                    positions = positionCalculator.Calculate(sourceDetail, calculationContext);
                }
                break;

                case ProductDateType.Quarter:
                {
                    positions = CalculateForQuartersMonths(
                        riskDate,
                        productDate,
                        sourceProduct,
                        calculationContext,
                        positionCalculator,
                        sourceDetail);
                }
                break;

                case ProductDateType.Year:
                {
                    positions = CalculateForCalsMonths(
                        riskDate,
                        productDate,
                        sourceProduct,
                        calculationContext,
                        positionCalculator,
                        sourceDetail);
                }
                break;

                case ProductDateType.Day:
                {
                    positions = _balmoPositionCalculator.Calculate(sourceDetail, calculationContext);
                }
                break;

                case ProductDateType.Custom:
                {
                    positions = CalculateForCustomPeriod(
                        riskDate,
                        productDate,
                        sourceProduct,
                        calculationContext,
                        positionCalculator,
                        sourceDetail);
                }
                break;

                case ProductDateType.Daily:
                {
                    positions = positionCalculator.Calculate(sourceDetail, calculationContext);
                }
                break;
            }

            return positions;
        }

        private static List<CalculationDetail> CalculateForQuartersMonths(
            DateTime riskDate,
            DateTime productDate,
            Product sourceProduct,
            CalculationContext calculationContext,
            IPositionCalculator positionCalculator,
            SourceDetail sourceDetail)
        {
            List<CalculationDetail> positions = new List<CalculationDetail>();

            for (int month = productDate.Month; month < productDate.Month + 3; month++)
            {
                if (IsNonFuturesBeforeRiskDate(sourceProduct.Type, riskDate, productDate.Year, month))
                {
                    continue;
                }

                CalculationContext context = calculationContext.GetNew(
                    null,
                    month,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    Product.NoDailyDiffMonthShift,
                    null,
                    null,
                    null,
                    null);
                positions.AddRange(positionCalculator.Calculate(sourceDetail, context));
            }

            return positions;
        }

        private static bool IsNonFuturesBeforeRiskDate(
            ProductType sourceProductType,
            DateTime riskDate,
            int productYear,
            int month)
        {
            bool isNonFutures = sourceProductType != ProductType.Futures;
            bool isYearStrictlyBeforeRiskDate = productYear < riskDate.Year;
            bool isYearOnOrBeforeRiskDate = productYear <= riskDate.Year;
            bool isMonthBeforeRiskDate = month < riskDate.Month;
            bool isProductDateBeforeRiskDate = (isMonthBeforeRiskDate && isYearOnOrBeforeRiskDate)
                                               || isYearStrictlyBeforeRiskDate;

            return isNonFutures && isProductDateBeforeRiskDate;
        }

        private static List<CalculationDetail> CalculateForCalsMonths(
            DateTime riskDate,
            DateTime productDate,
            Product sourceProduct,
            CalculationContext calculationContext,
            IPositionCalculator positionCalculator,
            SourceDetail sourceDetail)
        {
            List<CalculationDetail> positions = new List<CalculationDetail>();

            for (int month = 1; month <= 12; month++)
            {
                if (IsNonFuturesBeforeRiskDate(sourceProduct.Type, riskDate, productDate.Year, month))
                {
                    continue;
                }

                CalculationContext context = calculationContext.GetNew(
                    null,
                    month,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    Product.NoDailyDiffMonthShift,
                    null,
                    null,
                    null,
                    null);
                positions.AddRange(positionCalculator.Calculate(sourceDetail, context));
            }

            return positions;
        }

        private static List<CalculationDetail> CalculateForCustomPeriod(
            DateTime riskDate,
            DateTime productDate,
            Product sourceProduct,
            CalculationContext calculationContext,
            IPositionCalculator positionCalculator,
            SourceDetail sourceDetail)
        {
            List<CalculationDetail> positions = new List<CalculationDetail>();
            DateTime startDate = productDate;
            DateTime endDate = sourceDetail.TradeEndDate;

            int numMonths = endDate.MonthsSince(startDate) + 1;

            for (int i = 0; i < numMonths; i++)
            {
                DateTime date = startDate.AddMonths(i);
                int year = date.Year;
                int month = date.Month;

                if (IsNonFuturesBeforeRiskDate(sourceProduct.Type, riskDate, year, month))
                {
                    continue;
                }

                CalculationContext context = calculationContext.GetNew(
                    year,
                    month,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    Product.NoDailyDiffMonthShift,
                    null,
                    null,
                    null,
                    null);
                positions.AddRange(positionCalculator.Calculate(sourceDetail, context));
            }

            return positions;
        }

        private void HandlePrecalcDetailCalculationError(
            SourceDetail sourceDetails,
            string errorContextForLog,
            Action<SourceDetail> reportError,
            Exception ex)
        {
            sourceDetails.CantCalculatePositions = true;

            _logger.Error(
                ex,
                "ClassicPositionsCalculator.Calculate: {0} for trade [{1}]",
                errorContextForLog,
                sourceDetails.TradeCaptureId);

            reportError(sourceDetails);
        }

        private static Action<SourceDetail> ReportCalculationError(
            string errorMsg,
            object additionalErrorData)
        {
            return
                (srcDetail) =>
                    ErrorReportingHelper.GlobalQueue.Enqueue(
                        new Error(
                            "IRM Server",
                            ErrorType.DataError,
                            errorMsg,
                            srcDetail.TradeCaptureId.ToString(),
                            additionalErrorData,
                            ErrorLevel.Critical));
        }

        private static Action<SourceDetail> DoNothingWithSourceDetail(string ignoredMsg, object ignoredObject)
        {
            return (srcDetail) => { };
        }

        private static void SetSourceDetailLegToUse(SourceDetail sourceDetail, bool useSecondStrip)
        {
            if (sourceDetail != null)
            {
                if (useSecondStrip)
                {
                    sourceDetail.ChangeToSecondLeg();
                }
                else
                {
                    sourceDetail.ChangeToFirstLeg();
                }
            }
        }

        private Product GetSourceProduct(SourceDetail sourceDetail, int productId)
        {
            Product sourceProduct;

            if ((null != sourceDetail.Product) && (sourceDetail.Product.ProductId == productId))
            {
                sourceProduct = sourceDetail.Product;
            }
            else
            {
                sourceProduct = _calculationCache.GetProductById(productId);
            }
            return sourceProduct;
        }

        private List<CalculationDetail> Reduce(List<CalculationDetail> result)
        {
            List<CalculationDetail> reducedResults =
                result.Where(x => x != null)
                      .GroupBy(g => new { g.ProductId, g.SourceProductId, g.CalculationDate })
                      .Select(
                          g =>
                          {
                              CalculationDetail first = g.First();
                              decimal amount = g.Sum(y => y.Amount);

                              return new CalculationDetail
                              {
                                  DetailId = Guid.NewGuid(),
                                  CalculationDate = first.CalculationDate,
                                  Product = first.Product,
                                  ProductReference = first.ProductReference,
                                  Source = first.Source,
                                  ProductCategoryId = first.ProductCategoryId,
                                  ProductCategory = first.ProductCategory,
                                  ProductCategoryAbbreviation = first.ProductCategoryAbbreviation,
                                  Amount = amount,
                                  AmountInner = amount,
                                  ProductId = first.ProductId,
                                  SourceProductId = first.SourceProductId,
                                  ProductDate = first.ProductDate,
                                  ProductDateType = first.ProductDateType,
                                  MappingColumn = first.MappingColumn,
                                  OfficialProductId = first.OfficialProductId,
                                  PnlFactor = first.PnlFactor,
                                  PositionFactor = first.PositionFactor,
                                  DailyDetails =
                                      new ConcurrentBag<DailyDetail>(
                                          g.Where(x => x.DailyDetails != null).SelectMany(x => x.DailyDetails).ToList()),
                                  SourceDetails =
                                      g.Where(x => x.SourceDetails != null)
                                       .SelectMany(x => x.SourceDetails)
                                       .Distinct()
                                       .ToList(),
                                  SourceDetailAmountsDict =
                                      UnionDictionaries(
                                          g.Where(x => x.SourceDetailAmountsDict != null)
                                           .SelectMany(x => x.SourceDetailAmountsDict))
                              };
                          }).ToList();

            return reducedResults;
        }

        private ConcurrentDictionary<int, decimal> UnionDictionaries(IEnumerable<KeyValuePair<int, decimal>> listPairs)
        {
            ConcurrentDictionary<int, decimal> result = new ConcurrentDictionary<int, decimal>();

            foreach (KeyValuePair<int, decimal> pair in listPairs)
            {
                decimal amount;

                if (result.TryGetValue(pair.Key, out amount))
                {
                    decimal ignore;
                    result.TryRemove(pair.Key, out ignore);
                }

                amount += pair.Value;
                result.TryAdd(pair.Key, amount);
            }

            return result;
        }
    }
}