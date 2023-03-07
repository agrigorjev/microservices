using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Mandara.Extensions.Collections;
using Mandara.Business.Client.Extensions;

namespace Mandara.Entities.Calculation
{
    /// <summary>
    /// Perform record of incoming PnL data and calculation of normalized value
    /// 12-15-2012: Live PnL only in use.
    /// </summary>
    public class PnlThrottlingHelper
    {
        /// <summary>
        /// Z value determining limit for throttling input
        /// </summary>
        public static decimal Z_Ethalon
        {
            get
            {
                if (decimal.TryParse(ConfigurationManager.AppSettings["ThrottleZValueLimit"] ?? "10", out decimal zValue))
                {
                    return zValue;
                }

                return 10M;
            }
        }

        private static readonly decimal LiveToMeanPnLStdDeviationsLimit = Z_Ethalon;

        private const int PnLsRequiredForThrottleCalculation = 30;

        // store previous calculated value for each portfolio
        private ConcurrentDictionary<int, ConcurrentDictionary<string, decimal>> _throttledPnLHistory;
        // store 30 live pnl values for calculation purposes
        private ConcurrentDictionary<int, ConcurrentDictionary<string, ConcurrentQueue<decimal>>> _liveHistory;

        /// <summary>
        /// Initialize class
        /// </summary>
        public PnlThrottlingHelper()
        {
            _liveHistory = new ConcurrentDictionary<int, ConcurrentDictionary<string, ConcurrentQueue<decimal>>>();
            _throttledPnLHistory = new ConcurrentDictionary<int, ConcurrentDictionary<string, decimal>>();
        }

        /// <summary>
        /// Add updated pnl data to history record
        /// </summary>
        /// <param name="newPortfoliosPnLs">Dictionary of pnlData by portfolio identifier</param>
        public void Add(Dictionary<int, Dictionary<string, PnlData>> newPortfoliosPnLs)
        {
            foreach (KeyValuePair<int, Dictionary<string, PnlData>> portfolioPnLs in newPortfoliosPnLs)
            {
                int portfolio = portfolioPnLs.Key;
                Dictionary<string, PnlData> currencyPnLs = portfolioPnLs.Value;

                UpdatePortfolioCurrencyLivePnLsHistory(portfolio, currencyPnLs);
                GetThrottledLivePnLs(portfolio, currencyPnLs);
            }
        }

        private void UpdatePortfolioCurrencyLivePnLsHistory(
            int portfolio,
            Dictionary<string, PnlData> newCurrencyPnLs)
        {
            if (_liveHistory.TryGetValue(
                portfolio,
                out ConcurrentDictionary<string, ConcurrentQueue<decimal>> existingCurrencyPnLs))
            {
                AddCurrencyPnLsToExistingHistoryForPortfolio(newCurrencyPnLs, existingCurrencyPnLs);
            }
            else
            {
                AddCurrencyPnLsForPortfolioToHistory(portfolio, newCurrencyPnLs);
            }
        }

        private static void AddCurrencyPnLsToExistingHistoryForPortfolio(
            Dictionary<string, PnlData> newCurrencyPnLs,
            ConcurrentDictionary<string, ConcurrentQueue<decimal>> portfolioPnLs)
        {
            newCurrencyPnLs.Where(currencyPnL => currencyPnL.Value.LivePnl.HasValue).ForEach(currencyPnL =>
            {
                (string currency, PnlData pnl) = currencyPnL;

                if (!portfolioPnLs.TryGetValue(currency, out ConcurrentQueue<decimal> currencyQueue))
                {
                    currencyQueue = new ConcurrentQueue<decimal>();
                    portfolioPnLs.TryAdd(currency, currencyQueue);
                }

                currencyQueue.Enqueue(pnl.LivePnl.Value.Amount);

                if (currencyQueue.Count > PnLsRequiredForThrottleCalculation)
                {
                    currencyQueue.TryDequeue(out _);
                }
            });
        }

        private void AddCurrencyPnLsForPortfolioToHistory(int portfolio, Dictionary<string, PnlData> currencyPnLs)
        {
            _liveHistory.TryAdd(
                portfolio,
                CreateNewPortfolioPnLsHistory(
                    currencyPnLs.Where(portfolioPnL => portfolioPnL.Value.LivePnl.HasValue)));
        }

        private ConcurrentDictionary<string, ConcurrentQueue<decimal>> CreateNewPortfolioPnLsHistory(
            IEnumerable<KeyValuePair<string, PnlData>> livePnLs)
        {
            return new ConcurrentDictionary<string, ConcurrentQueue<decimal>>(
                livePnLs.ToDictionary(
                    portfolioPnL => portfolioPnL.Key,
                    portfolioPnL =>
                        new ConcurrentQueue<decimal>(new List<decimal>() { portfolioPnL.Value.LivePnl.Value.Amount })));
        }

        private void GetThrottledLivePnLs(int portfolio, Dictionary<string, PnlData> currencyPnLs)
        {
            foreach (KeyValuePair<string, ConcurrentQueue<decimal>> currencyPnLHistory in _liveHistory[portfolio])
            {
                string currency = currencyPnLHistory.Key;

                currencyPnLs.TryGetValue(currency, out PnlData pnlDataInput);
                GetThrottledLivePnL(pnlDataInput, portfolio, currency);
            }
        }

        private void GetThrottledLivePnL(PnlData pnlDataInput, int portfolio, string currency)
        {
            decimal livePnL = pnlDataInput.LivePnl ?? 0M;
            decimal throttledPnL = CalculateThrottledValue(portfolio, currency, livePnL);

            if (!_throttledPnLHistory.TryGetValue(
                portfolio,
                out ConcurrentDictionary<string, decimal> portfolioThrottledPnLs))
            {
                portfolioThrottledPnLs = new ConcurrentDictionary<string, decimal>();
                _throttledPnLHistory.TryAdd(portfolio, portfolioThrottledPnLs);
            }

            portfolioThrottledPnLs.AddOrUpdate(currency, livePnL, (key, _) => throttledPnL);
        }

        /// <summary>
        /// Performs statistic operations
        /// </summary>
        /// <param name="portfolioId">Affected portfolio ID (key)</param>
        /// <param name="livePnL">Value to throttle</param>
        /// <returns>Throttled value.</returns>
        private decimal CalculateThrottledValue(int portfolioId, string currency, decimal livePnL)
        {
            decimal[] pnls = GetLivePnLHistory(portfolioId, currency);

            if (pnls.Length >= PnLsRequiredForThrottleCalculation
                && _throttledPnLHistory.TryGetValue(
                    portfolioId,
                    out ConcurrentDictionary<string, decimal> throttlePnLs))
            {
                return GetThrottledLivePnL(currency, livePnL, pnls, throttlePnLs);
            }

            // in any other case return input value unchanged
            return livePnL;
        }

        private static decimal GetThrottledLivePnL(
            string currency,
            decimal livePnL,
            decimal[] pnls,
            ConcurrentDictionary<string, decimal> throttlePnLs)
        {
            decimal numDeviationsFromMean = CalculateStdDeviationsFromLivePnLToMean(livePnL, pnls);

            return Math.Abs(numDeviationsFromMean) > LiveToMeanPnLStdDeviationsLimit
                   && throttlePnLs.TryGetValue(currency, out decimal prevThrottledValue)
                ? prevThrottledValue
                : livePnL;
        }

        private static decimal CalculateStdDeviationsFromLivePnLToMean(decimal livePnL, decimal[] pnls)
        {
            decimal meanPnL = CalculateMeanLivePnL(pnls);
            double stdDev = CalculateLivePnLStdDeviation(pnls, meanPnL);

            return (double.IsNaN(stdDev) || ((decimal)stdDev) == 0M)
                ? 0
                : (livePnL - meanPnL) / (decimal)stdDev;
        }

        private static decimal CalculateMeanLivePnL(decimal[] pnls)
        {
            return pnls.Sum() / pnls.Length;
        }

        private static double CalculateLivePnLStdDeviation(decimal[] pnls, decimal meanPnL)
        {
            decimal sumOfSquaredDeviations = pnls.Sum(pnl => (pnl - meanPnL) * (pnl - meanPnL));
            decimal pnlVariance = sumOfSquaredDeviations / (pnls.Length - 1);

            return Math.Sqrt((double)pnlVariance);
        }

        private decimal[] GetLivePnLHistory(int portfolioId, string currency)
        {
            if (!_liveHistory.TryGetValue(portfolioId, out ConcurrentDictionary<string, ConcurrentQueue<decimal>> livePnLHistory)
                || !livePnLHistory.TryGetValue(currency, out ConcurrentQueue<decimal> pnlsHistory)
                || pnlsHistory.Count < PnLsRequiredForThrottleCalculation)
            {
                return new decimal[0];
            }

            return pnlsHistory.ToArray();
        }

        /// <summary>
        /// Get throttled value for given portfolio
        /// </summary>
        /// <param name="portfolioId">Portfolio identifier</param>
        /// <param name="currency"></param>
        /// <returns>Throttled live pnl value</returns>
        public Money GetThrottledValue(int portfolioId, string currency)
        {
            if (_throttledPnLHistory.TryGetValue(portfolioId, out ConcurrentDictionary<string, decimal> val))
            {
                if (val.TryGetValue(currency, out decimal pnl))
                {
                    return new Money(pnl, currency);
                }
            }

            return new Money(0M, currency);
        }
    }
}
