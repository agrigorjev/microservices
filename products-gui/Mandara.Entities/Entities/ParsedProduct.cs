using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Mandara.Entities
{
    [Table("ym_products")]
    public partial class ParsedProduct
    {
        [Column("ym_product_id")]
        [Key]
        public int ParsedProductId { get; set; }

        [Column("message_id")]
        public int MessageId { get; set; }

        [Column("official_product_id")]
        public int OfficialProductId { get; set; }

        [Column("product_date1", TypeName = "date")]
        public DateTime? Date1 { get; set; }

        [Column("product_date1_type")]
        public short? DateTypeDb1 { get; set; }

        [Column("product_date2", TypeName = "date")]
        public DateTime? Date2 { get; set; }

        [Column("product_date2_type")]
        public short? DateTypeDb2 { get; set; }

        [Column("price_bid", TypeName = "money")]
        public decimal? Bid { get; set; }

        [Column("price_ask", TypeName = "money")]
        public decimal? Ask { get; set; }

        [Column("price_mid", TypeName = "money")]
        public decimal? Mid { get; set; }

        [Column("mandara_price", TypeName = "money")]
        public decimal? MandaraPrice { get; set; }

        [Column("marked_incorrect")]
        public bool? MarkedIncorrect { get; set; }

        [Column("default_product_rules")]
        [StringLength(3000)]
        public string default_product_rules { get; set; }

        [Column("marked_for_alias_review")]
        public bool? marked_for_alias_review { get; set; }

        [ForeignKey("OfficialProductId")]
        public virtual OfficialProduct Product
        {
            get { return _product; }
            set
            {
                _product = value;
                OfficialProductId = _product != null ? _product.OfficialProductId : 0;
            }
        }

        [ForeignKey("MessageId")]
        public virtual YahooMessage Message
        {
            get { return _message; }
            set
            {
                _message = value;
                MessageId = _message != null ? _message.MessageId : 0;
            }
        }


        [NotMapped]
        public ProductDateType DateType1
        {
            get
            {
                return DateTypeDb1 == null ? ProductDateType.MonthYear : (ProductDateType)DateTypeDb1;
            }
            set
            {
                DateTypeDb1 = (Int16)value;
            }
        }

        [NotMapped]
        public ProductDateType DateType2
        {
            get
            {
                return DateTypeDb2 == null ? ProductDateType.MonthYear : (ProductDateType)DateTypeDb2;
            }
            set
            {
                DateTypeDb2 = (Int16)value;
            }
        }

        [NotMapped]
        public String DisplayDate1
        {
            get
            {
                return GetDisplayDate(Date1, DateTypeDb1);
            }
        }

        [NotMapped]
        public String DisplayDate2
        {
            get
            {
                return GetDisplayDate(Date2, DateTypeDb2);
            }
        }

        [NotMapped]
        public bool IsGrey
        {
            get
            {
                return MarkedIncorrect.HasValue && MarkedIncorrect.Value ? true : false;
            }
        }

        [NotMapped]
        public bool IsGreen
        {
            get
            {
                return !IsGrey && Bid.HasValue && MandaraPrice.HasValue && MandaraPrice < Bid;
            }
        }

        [NotMapped]
        public bool IsRed
        {
            get
            {
                return !IsGrey && Ask.HasValue && MandaraPrice.HasValue && MandaraPrice > Ask;
            }
        }

        private static decimal _threshold = 0.05M;
        private OfficialProduct _product;
        private YahooMessage _message;

        [NotMapped]
        public static decimal Threshold
        {
            get { return _threshold; }
            set { _threshold = value; }
        }

        [NotMapped]
        public bool IsBlue
        {
            get
            {
                if (IsRed || IsGreen || IsGrey)
                    return false;
                if (!MandaraPrice.HasValue)
                    return false;
                if (!Mid.HasValue)
                    return false;
                if (Mid.Value == 0)
                    return false;

                return Math.Abs(Math.Abs(MandaraPrice.Value / Mid.Value) - 1) > Threshold;
            }
        }

        [NotMapped]
        public bool IsFlagged
        {
            get { return IsGreen || IsRed || IsBlue; }
        }

        [NotMapped]
        public bool IsVHALProduct
        {
            get;
            set;
        }

        public ParsedProduct()
        {
            IsVHALProduct = false;
        }

        public ParsedProduct(HALProduct vhalProduct)
        {
            IsVHALProduct = true;
            ParsedProductId = vhalProduct.ProductId;
            Message = new YahooMessage
            {
                MessageBody = vhalProduct.VHALMessage.RawText,
                Broker = vhalProduct.VHALMessage.Broker,
                Received = vhalProduct.VHALMessage.Received,
                GroupName = "Fuel"
            };
            Product = vhalProduct.OfficialProduct;
            Date1 = vhalProduct.Date1;
            Date2 = vhalProduct.Date2;
            DateTypeDb1 = vhalProduct.DateTypeDb1;
            DateTypeDb2 = vhalProduct.DateTypeDb2;
            Bid = vhalProduct.Bid;
            Ask = vhalProduct.Ask;
            Mid = vhalProduct.Mid;
            MandaraPrice = vhalProduct.MandaraPrice;
            MarkedIncorrect = vhalProduct.MarkedIncorrect;
        }

        public string GetTextForSpeech(YahooMessage message)
        {
            if (!IsGreen && !IsRed)
                return String.Empty;

            var dateText = String.Format("{0} {1}", GetDisplayDateForSpeech(Date1, DateType1), GetDisplayDateForSpeech(Date2, DateType2));
            var price = IsGreen ? Bid : Ask;
            var priceText = GetPriceStringForSpeech(price.Value);
            var companyName = message.Broker.Company == null
                                  ? message.Broker.YahooId
                                  : message.Broker.Company.CompanyName;

            string textToSpeech = String.Format("{4} {0} {1} {2} {3}",
                Product.VoiceName ?? Product.DisplayName ?? Product.Name,
                IsGreen ? " BID " : " OFFERED ",
                priceText,
                companyName,
                dateText);

            return textToSpeech;
        }

        public static string GetDisplayDate(DateTime? date, short? dateType)
        {
            if (dateType == null)
                return String.Empty;

            return GetDisplayDate(date, (ProductDateType)dateType);
        }

        public static String GetDisplayDate(DateTime? date, ProductDateType dateType)
        {
            String displayDate = String.Empty;

            if (date == null)
                return displayDate;

            switch (dateType)
            {
                case ProductDateType.MonthYear:
                    displayDate = String.Format("{0} {1}", date.Value.ToString("MMM"), date.Value.Year);
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

        public static String GetDisplayDateForSpeech(DateTime? date, ProductDateType dateType)
        {
            String displayDate = String.Empty;

            if (date == null)
                return displayDate;

            switch (dateType)
            {
                case ProductDateType.MonthYear:
                    displayDate = String.Format("{0}", date.Value.ToString("MMM", CultureInfo.InvariantCulture));
                    break;
                case ProductDateType.Quarter:
                    Int32 quarter = date.Value.Month / 3 + 1;
                    displayDate = string.Format("Q{0}", quarter);
                    break;
                case ProductDateType.Year:
                    displayDate = string.Format("CAL{0}", date.Value.Year.ToString().Substring(2));
                    break;
                case ProductDateType.Day:
                    displayDate = date.Value.ToString("dd MMM", CultureInfo.InvariantCulture);
                    break;
            }
            return displayDate;
        }

        public static string GetPriceStringForSpeech(decimal price)
        {
            var priceText = price.ToString("##### #0.00", CultureInfo.InvariantCulture).Replace(".", "point").Replace("-", "minus").Trim();

            if (!priceText.StartsWith("0") && !priceText.StartsWith("point"))
            {
                priceText = priceText.Replace("point50", " and a half");
                priceText = priceText.Replace("point25", " and a quarter");
            }

            return priceText;
        }

    }
}
