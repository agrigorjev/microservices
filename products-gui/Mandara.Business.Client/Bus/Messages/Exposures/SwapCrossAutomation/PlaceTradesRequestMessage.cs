using System;
using System.Collections.Generic;
using System.Linq;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;
using Mandara.Entities.Enums;

namespace Mandara.Business.Bus.Messages.Exposures.SwapCrossAutomation
{
    public class PlaceTradesRequestMessage : MessageBase
    {
        private string _authorisedUserName;
        public string AuthorizedUsername
        {
            get => _authorisedUserName;
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"AuthorisedUserName cannot be empty.");
                }

                _authorisedUserName = value;
            }
        }

        private List<TradeCapture> _trades;
        public List<TradeCapture> Trades
        {
            get => _trades;
            set
            {
                if (null == value || !value.Any())
                {
                    throw new ArgumentException("No trades provided.");
                }

                _trades = new List<TradeCapture>(value);
            }
        }

        private List<SwapExposure> _swapExposures;
        public List<SwapExposure> SwapExposures
        {
            get => _swapExposures;
            set
            {
                if (null == value)
                {
                    throw new ArgumentNullException("Swap exposures cannot be null.");
                }

                _swapExposures = new List<SwapExposure>(value);
            }
        }

        private int _swapPortfolio;
        public int SwapPortfolio
        {
            get => _swapPortfolio;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException($"Swap portfolio ID {value} is not valid.");
                }

                _swapPortfolio = value;
            }
        }

        private int _futuresPortfolio;
        public int FuturesPortfolio
        {
            get => _futuresPortfolio;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException($"Futures portfolio ID {value} is not valid.");
                }

                _futuresPortfolio = value;
            }
        }

        public BaseMonth ExposureMonth { get; set; }
    }
}