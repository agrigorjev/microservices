using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mandara.Entities;

namespace Mandara.Entities.Parser
{
    public class Instrument
    {
        private decimal? _midPrice = null;

        public String OriginalMessage { get; set; }
        public OfficialProduct Product { get; set; }
        public DateTime? Date1 { get; set; }
        public ProductDateType DateType1 { get; set; }
        public DateTime? Date2 { get; set; }
        public ProductDateType DateType2 { get; set; }
        public Decimal? BidPrice { get; set; }
        public Decimal? AskPrice { get; set; }

        public bool AskHasPoint { get; set; }
        public bool BidHasPoint { get; set; }
        public bool MidHasPoint { get; set; }

        public Instrument Clone()
        {
            Instrument clone = new Instrument
                                   {
                                       OriginalMessage = this.OriginalMessage,
                                       Product = this.Product,
                                       Date1 = this.Date1,
                                       DateType1 = this.DateType1,
                                       Date2 = this.Date2,
                                       DateType2 = this.DateType2,
                                       BidPrice = this.BidPrice,
                                       AskPrice = this.AskPrice,
                                       MidPrice = this.MidPrice,
                                       AskHasPoint = this.AskHasPoint,
                                       BidHasPoint = this.BidHasPoint,
                                       MidHasPoint = this.MidHasPoint
                                   };
            return clone;
        }


        public Decimal? MidPrice 
        { 
            get
            {
                if (!_midPrice.HasValue && BidPrice.HasValue && AskPrice.HasValue)
                {
                    _midPrice = (AskPrice.Value + BidPrice.Value) / 2;
                }
                return _midPrice;
            }
            set
            {
                _midPrice = value;
            }
        }

        public String DisplayDate1
        {
            get
            {
                return GetDisplayDate(Date1, DateType1);
            }
        }

        public String DisplayDate2
        {
            get
            {
                return GetDisplayDate(Date2, DateType2);
            }
        }

        private String GetDisplayDate(DateTime? date, ProductDateType dateType)
        {
            String displayDate = String.Empty;
            
            if (date == null)
                return displayDate;

            switch (dateType)
            {
                case ProductDateType.MonthYear:
                    displayDate = String.Format("{0} {1}", date.Value.ToString("MMMM"), date.Value.Year);
                    break;
                case ProductDateType.Quarter:
                    Int32 quarter = date.Value.Month / 3 + 1;
                    displayDate = string.Format("Q{0} {1}", quarter, date.Value.Year);
                    break;
                case ProductDateType.Year:
                    displayDate = string.Format("CAL {0}", date.Value.Year);
                    break;
                case ProductDateType.Day:
                    displayDate = date.Value.ToString();
                    break;
            }
            return displayDate;
        }
    }
}
