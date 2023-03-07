using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Bus.Messages.Positions;
using Mandara.Entities.Calculation;

namespace Mandara.Business.Bus.Messages.ProductBreakdown
{
    public class ProductBreakdownResponseMessage : MessageBase
    {
        public List<ProductBreakdownItem> ProductBreakdownItems { get; set; }

        public Fees Fees { get; set; }

        public Holidays Holidays { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class ProductHoliday
    {
        public DateTime? Leg1Holiday { get; set; }
        public DateTime? Leg2Holiday { get; set; }
    }

    public class Holidays
    {
        public string Leg1ProductName { get; set; }
        public string Leg2ProductName { get; set; }
        public string Leg1CalendarName { get; set; }
        public string Leg2CalendarName { get; set; }
        public bool CommonPricing { get; set; }
        public List<ProductHoliday> HolidaysList { get; set; }
        public List<ProductExpirationDay> ExpirationDays { get; set; }
    }

    public class ProductExpirationDay
    {
        public DateTime ExpirationMonth { get; set; }
        public DateTime? Leg1ExpirationDate { get; set; }
        public DateTime? Leg2ExpirationDate { get; set; }
    }

    public class Fees
    {
        public decimal Clearing { get; set; }
        public decimal Block { get; set; }
        public decimal Commission { get; set; }
        public decimal Exchange { get; set; }
        public decimal Nfa { get; set; }
        public decimal Platts { get; set; }
        public decimal Cash { get; set; }
    }

    public class ProductBreakdownItem
    {
        public DateTime Day { get; set; }
        public decimal? LivePnl { get; set; }
        public List<CalculationDetailDto> Positions { get; set; }
        public decimal OvernightPnl { get; set; }
        public decimal? LivePrice { get; set; }
        public decimal? Settlement { get; set; }
        public decimal? Leg1Settlement { get; set; }
        public decimal? Leg2Settlement { get; set; }
    }
}