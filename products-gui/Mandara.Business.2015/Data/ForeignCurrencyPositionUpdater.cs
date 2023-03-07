using Mandara.Entities;
using Mandara.Entities.Entities;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Mandara.Extensions.Collections;

namespace Mandara.Business.Data
{
    public class ForeignCurrencyPositionsUpdater : IForeignCurrencyPositionsUpdater
    {
        private struct PositionId : IEquatable<PositionId>
        {
            public int PortfolioId { get; }
            public int CurrencyId { get; }

            public PositionId(int portfolio, int curr)
            {
                PortfolioId = portfolio;
                CurrencyId = curr;
            }

            public bool Equals(PositionId other)
            {
                return PortfolioId == other.PortfolioId && CurrencyId == other.CurrencyId;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                return obj is PositionId && Equals((PositionId)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (PortfolioId * 397) ^ CurrencyId;
                }
            }
        }

        private static readonly ILogger Logger = new NLogLoggerFactory().GetCurrentClassLogger();

        public void Replace(List<ForeignCurrencyPosition> positions, DateTime date)
        {
            if (!positions.Any())
            {
                return;
            }

            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                List<ForeignCurrencyPosition> dbPositions =
                    context.ForeignCurrencyPositions.Where(it => it.Date == date).ToList();

                context.ForeignCurrencyPositions.RemoveRange(dbPositions);

                if (positions.Any())
                {
                    context.ForeignCurrencyPositions.AddRange(positions);
                }

                context.SaveChanges();
            }
        }

        private static MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(ForeignCurrencyPositionsUpdater));
        }

        public void Update(List<ForeignCurrencyPosition> recalcPositions, DateTime date)
        {
            if (!recalcPositions.Any())
            {
                return;
            }

            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                Dictionary<PositionId, ForeignCurrencyPosition> dbPositions = context
                    .ForeignCurrencyPositions.Include(pos => pos.ForeignCurrencyPositionDetails)
                    .Where(pos => pos.Date == date)
                    .ToDictionary(pos => new PositionId(pos.PortfolioId, pos.CurrencyId), pos => pos);

                foreach (ForeignCurrencyPosition newPos in recalcPositions)
                {
                    ForeignCurrencyPosition dbPortfolioCurrPos;

                    if (dbPositions.TryGetValue(
                        new PositionId(newPos.PortfolioId, newPos.CurrencyId),
                        out dbPortfolioCurrPos))
                    {
                        Dictionary<int, decimal> tradesPositions = newPos
                            .ForeignCurrencyPositionDetails
                            .ToDictionary(det => det.TradeCaptureId, det => det.PositionValue);
                        HashSet<int> updatedTradeIds =
                            UpdateExistingPositions(dbPortfolioCurrPos, tradesPositions, context);

                        AddNewTradePositions(tradesPositions, updatedTradeIds, dbPortfolioCurrPos);
                    }
                    else
                    {
                        context.ForeignCurrencyPositions.Add(newPos);
                    }
                }

                context.SaveChanges();
            }
        }

        private static HashSet<int> UpdateExistingPositions(
            ForeignCurrencyPosition dbPortfolioCurrPos,
            Dictionary<int, decimal> tradesPositions,
            MandaraEntities context)
        {
            HashSet<int> updatedTradeIds = new HashSet<int>();

            dbPortfolioCurrPos.ForeignCurrencyPositionDetails.ForEach(
                oldPos =>
                {
                    decimal tradePos;

                    if (!tradesPositions.TryGetValue(oldPos.TradeCaptureId, out tradePos))
                    {
                        // TODO: Should probably remove the old value here
                        dbPortfolioCurrPos.TotalPositionPnLValue -= oldPos.PositionValue;
                        context.ForeignCurrencyPositionDetails.Remove(oldPos);
                        return;
                    }

                    dbPortfolioCurrPos.TotalPositionPnLValue += (tradePos - oldPos.PositionValue);
                    oldPos.PositionValue = tradePos;
                    updatedTradeIds.Add(oldPos.TradeCaptureId);
                });

            return updatedTradeIds;
        }

        private static void AddNewTradePositions(
            Dictionary<int, decimal> tradesPositions,
            HashSet<int> updatedTradeIds,
            ForeignCurrencyPosition existingPos)
        {
            tradesPositions.Where(tradePos => !updatedTradeIds.Contains(tradePos.Key))
                           .ForEach(
                               trPos =>
                               {
                                   ForeignCurrencyPositionDetail newTradePos = new ForeignCurrencyPositionDetail
                                   {
                                       ForeignCurrencyPosition = existingPos,
                                       PositionValue = trPos.Value,
                                       TradeCaptureId = trPos.Key
                                   };
                                   existingPos.ForeignCurrencyPositionDetails.Add(newTradePos);
                                   existingPos.TotalPositionPnLValue += trPos.Value;
                               });
        }

        [Obsolete("This method must be removed once we've found the bug causing IRM-725.")]
        public void Update(ForeignCurrencyPositionData recalcPositions, DateTime date)
        {
            if (!recalcPositions.Positions.Any())
            {
                return;
            }

            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                Dictionary<PositionId, ForeignCurrencyPosition> dbPositions = context
                    .ForeignCurrencyPositions.Include(pos => pos.ForeignCurrencyPositionDetails)
                    .Where(pos => pos.Date == date)
                    .ToDictionary(pos => new PositionId(pos.PortfolioId, pos.CurrencyId), pos => pos);

                foreach (ForeignCurrencyPosition newPos in recalcPositions.Positions)
                {
                    ForeignCurrencyPosition dbPortfolioCurrPos;

                    if (dbPositions.TryGetValue(
                        new PositionId(newPos.PortfolioId, newPos.CurrencyId),
                        out dbPortfolioCurrPos))
                    {
                        Dictionary<int, decimal> tradesPositions = newPos
                            .ForeignCurrencyPositionDetails
                            .ToDictionary(det => det.TradeCaptureId, det => det.PositionValue);
                        HashSet<int> updatedTradeIds = UpdateExistingPositionsWithLogging(
                            dbPortfolioCurrPos,
                            GetLivePnLInputs(recalcPositions, newPos),
                            tradesPositions,
                            context);

                        AddNewTradePositionsWithLogging(
                            tradesPositions,
                            GetLivePnLInputs(recalcPositions, newPos),
                            updatedTradeIds,
                            dbPortfolioCurrPos);
                    }
                    else
                    {
                        context.ForeignCurrencyPositions.Add(newPos);
                        GetLivePnLInputs(recalcPositions, newPos)
                            .ForEach(pnlInput => Logger.Trace(
                                "New foreign currency position: {0}",
                                pnlInput.CalculatedPriceData));
                    }
                }

                context.SaveChanges();
            }
        }

        private static List<LivePnL> GetLivePnLInputs(
            ForeignCurrencyPositionData recalcPositions,
            ForeignCurrencyPosition newPos)
        {
            return recalcPositions.PositionsDetails.Where(
                                      posDetails =>
                                      {
                                          ForeignCurrencyPosition pos = posDetails.Item1.ForeignCurrencyPosition;

                                          return new PositionId(pos.PortfolioId, pos.CurrencyId).Equals(
                                              new PositionId(newPos.PortfolioId, newPos.CurrencyId));
                                      })
                                  .Select(posDetail => posDetail.Item2)
                                  .ToList();
        }

        private const string ChangedPositionLogTemplate =
            "Changing foreign currency position details for trade{0} - position contribution changed from {1} to {2}.  "
                + "Calculation inputs: {3}";

        private static HashSet<int> UpdateExistingPositionsWithLogging(
            ForeignCurrencyPosition dbPortfolioCurrPos,
            List<LivePnL> pnlInputs,
            Dictionary<int, decimal> tradesPositions,
            MandaraEntities context)
        {
            HashSet<int> updatedTradeIds = new HashSet<int>();

            dbPortfolioCurrPos.TotalPositionPnLValue = 0M;

            dbPortfolioCurrPos.ForeignCurrencyPositionDetails.ForEach(
                oldPos =>
                {
                    decimal tradePos;

                    if (!tradesPositions.TryGetValue(oldPos.TradeCaptureId, out tradePos))
                    {
                        Logger.Trace(
                            "Removing foreign currency position details for trade {0} - position contribution was {1}",
                            oldPos.TradeCaptureId,
                            oldPos.PositionValue);

                        context.ForeignCurrencyPositionDetails.Remove(oldPos);
                        return;
                    }

                    if (oldPos.PositionValue != tradePos)
                    {
                        Logger.Trace(
                            ChangedPositionLogTemplate,
                            oldPos.TradeCaptureId,
                            oldPos.PositionValue,
                            tradePos,
                            pnlInputs.First(pnlInput => pnlInput.CalculatedPriceData.TradeId == oldPos.TradeCaptureId)
                                     .CalculatedPriceData);
                        dbPortfolioCurrPos.TotalPositionPnLValue += tradePos;
                        oldPos.PositionValue = tradePos;
                        updatedTradeIds.Add(oldPos.TradeCaptureId);
                    }
                    else
                    {
                        dbPortfolioCurrPos.TotalPositionPnLValue += oldPos.PositionValue;
                    }
                });

            return updatedTradeIds;
        }

        private static void AddNewTradePositionsWithLogging(
            Dictionary<int, decimal> tradesPositions,
            List<LivePnL> pnlInputs,
            HashSet<int> updatedTradeIds,
            ForeignCurrencyPosition existingPos)
        {
            tradesPositions.Where(tradePos => !updatedTradeIds.Contains(tradePos.Key))
                           .ForEach(
                               tradePos =>
                               {
                                   Logger.Trace(
                                       "Added foreign currency position {0} from inputs [{1}]",
                                       tradePos.Value,
                                       pnlInputs.First(pnlInput => pnlInput.CalculatedPriceData.TradeId == tradePos.Key)
                                                .CalculatedPriceData);

                                   ForeignCurrencyPositionDetail newTradePos = new ForeignCurrencyPositionDetail
                                   {
                                       ForeignCurrencyPosition = existingPos,
                                       PositionValue = tradePos.Value,
                                       TradeCaptureId = tradePos.Key
                                   };
                                   existingPos.ForeignCurrencyPositionDetails.Add(newTradePos);
                                   existingPos.TotalPositionPnLValue += tradePos.Value;
                               });
        }
    }
}
