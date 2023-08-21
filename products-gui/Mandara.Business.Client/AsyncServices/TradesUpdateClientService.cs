using Mandara.Business.Bus;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages.Trades;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Mandara.Entities.ErrorReporting;
using Mandara.Extensions.Option;
using Ninject.Extensions.Logging;
using System.Linq;

namespace Mandara.Business.AsyncServices
{
    public class TradesUpdateClientService : QueuePollingAsyncService<TradesUpdateMessage>
    {
        public TradesUpdateClientService(CommandManager commandManager, BusClient busClient, ILogger log)
            : base(commandManager, busClient, log, busClient.PendingTradesUpdates)
        {
        }

        protected override bool IsQueueReady()
        {
            return BusClient.Portfolios.Any();
        }

        protected override void DoWork()
        {
            if (!IsQueueReady())
            {
                SleepForQueue();
                return;
            }

            TryGetResult<TradesUpdateMessage> message = PollForNextElement();
            if (!message.HasValue)
            {
                return;
            }

            if (message.Value.TradeCaptures != null)
            {
                foreach (TradeCapture newTradeCapture in message.Value.TradeCaptures)
                {
                    if (newTradeCapture.Portfolio != null)
                    {
                        if (BusClient.Portfolios.ContainsKey(newTradeCapture.Portfolio.PortfolioId))
                        {
                            newTradeCapture.Portfolio = BusClient.Portfolios[newTradeCapture.Portfolio.PortfolioId];
                        }
                        else
                        {
                            ErrorReportingHelper.GlobalQueue.Enqueue(
                                new Error("Bus Client", ErrorType.DataError,
                                            "Could not find portfolio with id: " + newTradeCapture.Portfolio.PortfolioId,
                                            newTradeCapture.Portfolio.PortfolioId.ToString(), null, ErrorLevel.Critical));
                        }
                    }

                    if (newTradeCapture.SellBook != null)
                    {
                        if (BusClient.Portfolios.ContainsKey(newTradeCapture.SellBook.PortfolioId))
                        {
                            newTradeCapture.SellBook = BusClient.Portfolios[newTradeCapture.SellBook.PortfolioId];
                        }
                        else
                        {
                            ErrorReportingHelper.GlobalQueue.Enqueue(
                                new Error("Bus Client", ErrorType.DataError,
                                            "Could not find portfolio with id: " + newTradeCapture.SellBook.PortfolioId,
                                            newTradeCapture.SellBook.PortfolioId.ToString(), null, ErrorLevel.Critical));
                        }
                    }

                    if (newTradeCapture.BuyBook != null)
                    {
                        if (BusClient.Portfolios.ContainsKey(newTradeCapture.BuyBook.PortfolioId))
                        {
                            newTradeCapture.BuyBook = BusClient.Portfolios[newTradeCapture.BuyBook.PortfolioId];
                        }
                        else
                        {
                            ErrorReportingHelper.GlobalQueue.Enqueue(
                                new Error("Bus Client", ErrorType.DataError,
                                            "Could not find portfolio with id: " + newTradeCapture.BuyBook.PortfolioId,
                                            newTradeCapture.BuyBook.PortfolioId.ToString(), null, ErrorLevel.Critical));
                        }
                    }

                    newTradeCapture.MessageType = TradeCaptureMessageType.Update;
                    BusClient.OnTradesChanged(newTradeCapture);
                }
            }


        }
    }
}