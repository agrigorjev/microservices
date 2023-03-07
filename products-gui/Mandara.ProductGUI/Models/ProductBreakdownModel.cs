using Mandara.Business.Bus.Messages.Positions;
using Mandara.Business.Bus.Messages.ProductBreakdown;
using Mandara.Entities;
using Ninject.Infrastructure.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Mandara.ProductGUI.Models
{
    public class MonthItem
    {
        public DateTime DateTime { get; set; }

        public override string ToString()
        {
            return DateTime.ToString("MMMyy");
        }
    }

    public class BreakdownItem
    {
        public DateTime Day { get; set; }

        public decimal? LivePnl { get; set; }

        public decimal Overnight { get; set; }

        public decimal? LivePrice { get; set; }

        public decimal? SettlementPrice { get; set; }

        public decimal? Leg1 { get; set; }

        public decimal? Leg2 { get; set; }
    }

    public class ProductBreakdownModel : INotifyPropertyChanged
    {
        private BreakdownItem _currentBreakdownItem;
        private OfficialProduct _currentOfficialProduct;
        private decimal _pnl;
        private decimal _sumLive;
        private decimal _sumOvernight;
        private decimal _avgSettle;
        private MonthItem _contractMonth;
        private DateTime _purchaseDay;
        private DateTime _startDay;
        private DateTime _endDay;
        private decimal _quantity;
        private decimal _startPrice;
        private Product _product;
        private bool _startDayApplicable;
        private bool _endDayApplicable;
        private bool _breakdownItemLegsApplicable;

        public ProductBreakdownModel()
        {
            BreakdownItems = new BindingList<BreakdownItem>();
            BreakdownItems.ListChanged += BreakdownItems_ListChanged;

            CurrentCalculationDetails = new BindingList<CalculationDetailDto>();
            CurrentProducts = new BindingList<Product>();
            OfficialProducts = new BindingList<OfficialProduct>();

            CalculationDetailsAll = new Dictionary<BreakdownItem, CalculationDetailDto[]>();

            DateTime now = DateTime.Now;

            ContractMonthItems = new List<MonthItem>();
            DateTime currMonth = new DateTime(now.Year, now.Month, 1);
            for (int i = 1; i < 13; i++)
            {
                DateTime month = currMonth.AddMonths(-i);
                ContractMonthItems.Add(new MonthItem() { DateTime = month });
            }

            ContractMonth = ContractMonthItems[0];
            PurchaseDay = now;
            StartDay = now;
            EndDay = now;
        }

        void BreakdownItems_ListChanged(object sender, ListChangedEventArgs e)
        {
            var items = (BindingList<BreakdownItem>)sender;
            decimal sumLive = 0;
            decimal sumOvernight = 0;
            decimal sumSettle = 0;
            int settlementDays = 0;

            HashSet<DateTime> holidays = new HashSet<DateTime>();
            DateTime t, value;
            if (Holidays != null)
            {
                foreach (var productHoliday in Holidays.HolidaysList)
                {
                    if (productHoliday.Leg1Holiday != null)
                    {
                        value = productHoliday.Leg1Holiday.Value;
                        t = new DateTime(value.Year, value.Month, value.Day);
                        holidays.Add(t);
                    }
                    if (productHoliday.Leg2Holiday != null)
                    {
                        value = productHoliday.Leg2Holiday.Value;
                        t = new DateTime(value.Year, value.Month, value.Day);
                        holidays.Add(t);
                    }
                }
            }

            foreach (var item in items)
            {
                sumLive += item.LivePnl ?? 0;
                sumOvernight += item.Overnight;
                sumSettle += (item.SettlementPrice ?? 0M);

                t = new DateTime(item.Day.Year, item.Day.Month, item.Day.Day);
                if (!holidays.Contains(t))
                {
                    settlementDays++;
                }
            }

            SumLive = sumLive;
            SumOvernight = sumOvernight;

            //AvgSettle = items.Count == 0 ? 0 : sumSettle/items.Count;
            AvgSettle = settlementDays == 0 ? 0 : sumSettle / settlementDays;

            Pnl = SumLive + SumOvernight;
        }

        public Dictionary<BreakdownItem, CalculationDetailDto[]> CalculationDetailsAll { get; private set; }

        public List<ProductCategory> ProductCategories { get; set; }

        public Fees Fees { get; set; }

        public Holidays Holidays { get; set; }


        public string ErrorMessage { get; set; }

        #region Observable properties
        public bool StartDayApplicable
        {
            get { return _startDayApplicable; }
            private set
            {
                if (value.Equals(_startDayApplicable))
                    return;

                _startDayApplicable = value;
                OnPropertyChanged("StartDayApplicable");
            }
        }

        public bool EndDayApplicable
        {
            get { return _endDayApplicable; }
            private set
            {
                if (value.Equals(_endDayApplicable))
                    return;

                _endDayApplicable = value;
                OnPropertyChanged("EndDayApplicable");
            }
        }

        public bool BreakdownItemLegsApplicable
        {
            get { return _breakdownItemLegsApplicable; }
            private set
            {
                if (value.Equals(_breakdownItemLegsApplicable))
                    return;

                _breakdownItemLegsApplicable = value;
                OnPropertyChanged("BreakdownItemLegsApplicable");
            }
        }

        public decimal Pnl
        {
            get { return _pnl; }
            private set
            {
                if (value.Equals(_pnl))
                    return;

                _pnl = value;
                OnPropertyChanged("Pnl");
            }
        }

        public decimal SumLive
        {
            get { return _sumLive; }
            private set
            {
                if (value.Equals(_sumLive))
                    return;

                _sumLive = value;
                OnPropertyChanged("Pnl");
            }
        }

        public decimal SumOvernight
        {
            get { return _sumOvernight; }
            private set
            {
                if (value.Equals(_sumOvernight))
                    return;

                _sumOvernight = value;
                OnPropertyChanged("SumOvernight");
            }
        }

        public decimal AvgSettle
        {
            get { return _avgSettle; }
            private set
            {
                if (value.Equals(_avgSettle))
                    return;

                _avgSettle = value;
                OnPropertyChanged("AvgSettle");
            }
        }

        public MonthItem ContractMonth
        {
            get { return _contractMonth; }
            set
            {
                if (value.Equals(_contractMonth))
                    return;

                _contractMonth = value;
                OnPropertyChanged("ContractMonth");
            }
        }

        public List<MonthItem> ContractMonthItems { get; private set; }

        public DateTime PurchaseDay
        {
            get { return _purchaseDay; }
            set
            {
                if (value.Equals(_purchaseDay))
                    return;

                _purchaseDay = value;
                OnPropertyChanged("PurchaseDay");
            }
        }

        public DateTime StartDay
        {
            get { return _startDay; }
            set
            {
                if (value.Equals(_startDay))
                    return;

                _startDay = value;
                OnPropertyChanged("StartDay");
            }
        }

        public DateTime EndDay
        {
            get { return _endDay; }
            set
            {
                if (value.Equals(_endDay))
                    return;

                _endDay = value;
                OnPropertyChanged("EndDay");
            }
        }

        public decimal Quantity
        {
            get { return _quantity; }
            set
            {
                if (value.Equals(_quantity))
                    return;

                _quantity = value;
                OnPropertyChanged("Quantity");
            }
        }

        public decimal StartPrice
        {
            get { return _startPrice; }
            set
            {
                if (value.Equals(_startPrice))
                    return;

                _startPrice = value;
                OnPropertyChanged("StartPrice");
            }
        }

        public Product CurrentProduct
        {
            get { return _product; }
            set
            {
                if (object.Equals(value, _product))
                    return;

                _product = value;
                bool isDailyProduct = _product.Type.IsDaily();

                StartDayApplicable = _product.Type == ProductType.Balmo || isDailyProduct;
                EndDayApplicable = isDailyProduct;
                BreakdownItemLegsApplicable = _product.Type.IsDailyOrWeeklyDiff() || _product.Type == ProductType.Diff;

                OnPropertyChanged("CurrentProduct");
            }
        }

        public BreakdownItem CurrentBreakdownItem
        {
            get { return _currentBreakdownItem; }
            set
            {
                if (object.Equals(value, _currentBreakdownItem))
                    return;

                _currentBreakdownItem = value;

                if (_currentBreakdownItem == null)
                    return;

                CurrentCalculationDetails.Clear();
                if (CalculationDetailsAll.ContainsKey(_currentBreakdownItem))
                {
                    foreach (var calculationDetails in CalculationDetailsAll[_currentBreakdownItem])
                    {
                        CurrentCalculationDetails.Add(calculationDetails);
                    }
                }
                CurrentCalculationDetails.ResetBindings();

                OnPropertyChanged("CurrentBreakdownItem");
            }
        }

        public OfficialProduct CurrentOfficialProduct
        {
            get { return _currentOfficialProduct; }
            set
            {
                if (object.Equals(value, _currentOfficialProduct))
                    return;

                _currentOfficialProduct = value;

                CurrentProducts.Clear();
                ProductCategories.SelectMany(it => it.Products)
                    .Where(it => it.OfficialProductId == value.OfficialProductId).Map(CurrentProducts.Add);
                CurrentProducts.ResetBindings();

                OnPropertyChanged("CurrentOfficialProduct");
            }
        }

        public BindingList<BreakdownItem> BreakdownItems { get; private set; }

        public BindingList<OfficialProduct> OfficialProducts { get; private set; }

        public BindingList<CalculationDetailDto> CurrentCalculationDetails { get; private set; }

        public BindingList<Product> CurrentProducts { get; private set; }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
