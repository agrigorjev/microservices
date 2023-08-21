using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Enums;
using Newtonsoft.Json;

namespace Mandara.Business.Bus.Messages.Exposures.SwapCrossAutomation
{
    public class TransferTradesRequestMessage : MessageBase
    {
        public int SourcePortfolioId { get; private set; }
        public int DestinationPortfolioId { get; private set; }
        public DateTime CurrentMonth { get; private set; }
        public List<SwapExposure> SwapExposures { get; private set; }
        public BaseMonth BaseMonth { get; private set; }

        public TransferTradesRequestMessage()
        {
            SwapExposures = new List<SwapExposure>();
        }

        [JsonConstructor]
        public TransferTradesRequestMessage(
            int sourcePortfolioId,
            int destinationPortfolioId,
            DateTime currentMonth,
            List<SwapExposure> swapExposures,
            BaseMonth baseMonth)
        {
            SourcePortfolioId = sourcePortfolioId;
            DestinationPortfolioId = destinationPortfolioId;
            CurrentMonth = currentMonth;
            SwapExposures = swapExposures ?? new List<SwapExposure>();
            BaseMonth = baseMonth;
        }
    }
}