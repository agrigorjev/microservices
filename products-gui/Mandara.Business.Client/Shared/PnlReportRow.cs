using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;
using Mandara.Entities;
using Mandara.Entities.Enums;

namespace Mandara.Business
{
    public class PnlReportRow : INotifyPropertyChanged
    {
        public string NumberFormat { get; set; }

        private Money? _livePnlValue;

        public Money? LivePnlValue
        {
            get
            {
                return _livePnlValue;
            }
            set
            {
                if (_livePnlValue != null && value != null && _livePnlValue.Value.Equals(value.Value))
                    return;

                _livePnlValue = value;
                OnPropertyChanged("LivePnl");
                OnPropertyChanged("Total");
            }
        }

        public void SetLivePnlValue(Money? val)
        {
            _livePnlValue = val;
        }

        private Money? _overnightPnlValue;

        public Money? OvernightPnlValue
        {
            get
            {
                return _overnightPnlValue;
            }
            set
            {
                if (_overnightPnlValue != null && value != null && _overnightPnlValue.Value.Equals(value.Value))
                    return;

                _overnightPnlValue = value;
                OnPropertyChanged("OvernightPnl");
                OnPropertyChanged("Total");
            }
        }

        public void SetOvernightPnlValue(Money? val)
        {
            _overnightPnlValue = val;
        }

        private const String progressMessage = "Calculating...";

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, e);
        }

        public string LivePnl
        {
            get
            {
                if (_livePnlValue.HasValue)
                {
                    return _livePnlValue.Value.ToString(NumberFormat);
                }
                else
                {
                    return progressMessage;
                }
            }
        }
        //12/21/2012 returns direct OnPnl w/o/ any changes
        public string OvernightPnl
        {
            get
            {
                return _overnightPnlValue.HasValue ? GetOvernightPnl().ToString(NumberFormat) : progressMessage;
            }
        }
        public string Total
        {
            get
            {
                if (!_livePnlValue.HasValue || !_overnightPnlValue.HasValue)
                    return progressMessage;

                Money totalPnl = (_livePnlValue.Value + _overnightPnlValue.Value);
                return totalPnl.ToString(NumberFormat);
            }
        }



        public int PortfolioId { get; set; }
        public string Currency { get; set; }
        public int CurrencyId { get; set; }

        public Portfolio Portfolio { get; set; }

        public int ParentID
        {
            get { return (Portfolio != null ? Portfolio.ParentPortfolioId : PortfolioId) * 10000 + CurrencyId; }
        }

        public int ID
        {
            get { return (Portfolio != null ? Portfolio.PortfolioId : PortfolioId) * 10000 + CurrencyId; }
        }

        public string BookName
        {
            get { return Portfolio != null ? GetFullPortfolioName(Portfolio) : PortfolioId.ToString(CultureInfo.InvariantCulture); }
        }

        public PnlReportRow()
        {
            NumberFormat = "N2";
        }

        public PnlReportRow(Portfolio p)
        {
            NumberFormat = "N2";
            Portfolio = p;
            PortfolioId = p != null ? p.PortfolioId : 0;
        }

        public Money GetLivePnlValue()
        {
            return _livePnlValue ?? new Money(0M, Currency ?? CurrencyCodes.USD);
        }

        public Money GetTotalPnl()
        {
            if (_livePnlValue.HasValue && _overnightPnlValue.HasValue)
                return _livePnlValue.Value + _overnightPnlValue.Value;

            if (_livePnlValue.HasValue)
                return _livePnlValue.Value;

            if (_overnightPnlValue.HasValue)
                return _overnightPnlValue.Value;

            return new Money(0M, Currency ?? CurrencyCodes.USD);
        }

        public Money GetOvernightPnl()
        {
            return _overnightPnlValue ?? new Money(0M, Currency ?? CurrencyCodes.USD);
        }
        /// <summary>
        /// Retutn nont nullavle live global cost (liveCost +brokerage)
        /// </summary>
        /// <returns></returns>
        public Money GetLiveGlobalCost()
        {
            if (_liveCostsValue.HasValue && _brokerageValue.HasValue)
                return _liveCostsValue.Value + _brokerageValue.Value;

            if (_liveCostsValue.HasValue)
                return _liveCostsValue.Value;

            if (_brokerageValue.HasValue)
                return _brokerageValue.Value;

            return new Money(0M, Currency ?? CurrencyCodes.USD);
        }

        public static string GetFullPortfolioName(Portfolio portfolio)
        {
            if (portfolio == null)
                return string.Empty;
            var p = portfolio;
            var path = new List<string>();

            do
            {
                path.Insert(0, p.Name);
                p = p.ParentPortfolio;
            } while (p != null && p.PortfolioId != -1);

            return string.Join(" / ", path);
        }

        #region Fee
        private Money? _liveCostsValue;
        public Money? LiveCostsValue
        {
            get
            {
                return _liveCostsValue;
            }
            set
            {
                _liveCostsValue = value;
                OnPropertyChanged("LiveCosts");
            }
        }

        public string LiveCosts
        {
            get
            {
                return string.Format("-{0}", (LiveCostsValue ?? new Money(0M, Currency ?? CurrencyCodes.USD)).ToString(NumberFormat));
            }
        }

        private Money? _brokerageValue;
        //Strore brockerage
        public Money? BrokerageValue
        {
            get
            {
                return _brokerageValue;
            }
            set
            {
                _brokerageValue = value;
                OnPropertyChanged("Brokerage");
            }
        }
        public string Brokerage
        {
            get
            {
                return string.Format("-{0}", (BrokerageValue ?? new Money(0M, Currency ?? CurrencyCodes.USD)).ToString(NumberFormat));
            }
        }


        #endregion Fee
        #region Calculated

        //live global costs, liveCosts + brockerage
        public string LiveGlobal
        {
            get
            {
                return string.Format("-{0}", GetLiveGlobalCost().ToString(NumberFormat));
            }
        }
        //liveCosts
        public string LivePnlNet
        {
            get
            {
                if (!_livePnlValue.HasValue)
                    return progressMessage;
                Money livePnlNet = (_livePnlValue.Value - GetLiveGlobalCost());
                return livePnlNet.ToString(NumberFormat);
            }
        }
        //totla - livecosts global
        public string TotalPnlNet
        {
            get
            {
                if (!_livePnlValue.HasValue || !_overnightPnlValue.HasValue)
                    return progressMessage;

                Money totalPnl = (_livePnlValue.Value + _overnightPnlValue.Value) - GetLiveGlobalCost();
                return totalPnl.ToString(NumberFormat);
            }
        }



        #endregion

        public bool IsZeroRow()
        {
            if (!_livePnlValue.HasValue || !_overnightPnlValue.HasValue)
                return false;

            List<Money?> values = new List<Money?>
            {
                _livePnlValue,
                _overnightPnlValue,
                _liveCostsValue,
                _brokerageValue
            };

            if (values.TrueForAll(x => x.HasValue && Math.Abs(x.Value.Amount) >= 0M && Math.Abs(x.Value.Amount) < 0.000001M))
                return true;

            return false;
        }
    }

    public class PnlReportRowData
    {
        public int PortfolioId { get; set; }

        public string Currency { get; set; }

        public Money? LivePnl { get; set; }

        public Money? OvernightPnl { get; set; }

        public Money? LiveCosts { get; set; }

        public Money? Brokerage { get; set; }

        [XmlIgnore]
        public Money Total 
        {
            get
            {
                if (LivePnl.HasValue && OvernightPnl.HasValue)
                    return LivePnl.Value + OvernightPnl.Value;

                if (LivePnl.HasValue)
                    return LivePnl.Value;

                if (OvernightPnl.HasValue)
                    return OvernightPnl.Value;

                return new Money(0M, Currency ?? CurrencyCodes.USD);
            } 
        }
    }
}
