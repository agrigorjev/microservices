using System;
using System.ComponentModel;
using Mandara.Business.Bus.Messages.TradeAdd;
using Mandara.Entities;
using Mandara.Entities.Enums;

namespace Mandara.Business.TradeAdd
{
    public class StripDetail : INotifyPropertyChanged
    {
        public event InstrumentChangedEventHandler InstrumentChanged;

        private TradeAddDetails _tradeAddDetails;

        private decimal? _livePrice;
        private Instrument _instrument;
        private Unit _unit;
        private decimal _volume;
        private decimal _price;
        private Strip _strip;
        private decimal _leg1Price;
        private decimal _leg2Price;

        public StripDetail(TradeAddDetails tradeAddDetails)
        {
            _tradeAddDetails = tradeAddDetails;
        }

        public void SetTradeAddDetails(TradeAddDetails tradeAddDetails)
        {
            _tradeAddDetails = tradeAddDetails;
        }

        public Strip Strip
        {
            get { return _strip; }
            set
            {
                if (_strip == null && value == null)
                    return;

                bool changed = _strip == null || value == null;

                if (!changed)
                {
                    changed = _strip.StringValue != value.StringValue;
                }

                _strip = value;
                
                if (changed)
                {
                    OnPropertyChanged("Strip");
                    FireImpactChange();
                }
            }
        }

        public Instrument Instrument
        {
            get { return _instrument; }
            set
            {
                if (_instrument == null && value == null)
                    return;

                bool changed = _instrument == null || value == null;

                if (!changed)
                {
                    changed = _instrument.Id != value.Id;
                }

                _instrument = value;

                if (changed)
                {
                    OnPropertyChanged("Instrument");
                    FireInstrumentChange();
                    FireImpactChange();
                }
            }
        }

        public decimal Volume
        {
            get { return _volume; }
            set
            {
                bool changed = _volume != value;
                _volume = value;
                if (changed)
                {
                    OnPropertyChanged("Volume");
                    FireImpactChange();
                }
            }
        }

        public decimal Price
        {
            get { return _price; }
            set
            {
                bool changed = _price != value;
                _price = value;

                if (changed)
                {
                    OnPropertyChanged("Price");
                    CalculateLeg2Price();
                    FireImpactChange();
                }
            }
        }

        public Unit Unit
        {
            get { return _unit; }
            set
            {
                bool changed = _unit != value;
                _unit = value;

                if (changed)
                {
                    OnPropertyChanged("Unit");
                    FireImpactChange();
                }
            }
        }

        public decimal? LivePrice
        {
            get { return _livePrice; }
            set
            {
                bool changed = _livePrice != value;
                _livePrice = value;

                if (changed)
                {
                    OnPropertyChanged("LivePrice");
                }
            }
        }

        public decimal Leg1Price
        {
            get { return _leg1Price; }
            set
            {
                bool changed = _leg1Price != value;

                _leg1Price = value;

                if (changed)
                {
                    OnPropertyChanged("Leg1Price");
                    CalculateLeg2Price();
                    FireImpactChange();
                }
            }
        }

        private void CalculateLeg2Price()
        {
            _leg2Price = _leg1Price - Price;
        }

        public decimal Leg2Price
        {
            get { return _leg2Price; }
            set
            {
                bool changed = _leg2Price != value;

                _leg2Price = value;

                if (changed)
                {
                    OnPropertyChanged("Leg2Price");
                }
            }
        }

        private void FireInstrumentChange()
        {
            InstrumentChangedEventHandler handler = InstrumentChanged;

            if (handler != null)
                handler(this, new InstrumentChangedEventArgs());
        }

        private void FireImpactChange()
        {
            if (_tradeAddDetails != null)
                _tradeAddDetails.FireImpactChange();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            string stripName = null;

            if (Strip != null)
                stripName = Strip.StringValue;

            string unitString = Unit == null ? string.Empty : Unit.ToString();

            return string.Format("{0} {1:N2} {2}", stripName, Volume, unitString);
        }
    }
}