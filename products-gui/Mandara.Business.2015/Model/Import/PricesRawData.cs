using System;
using Mandara.Date;

namespace Mandara.Import
{
    public class PricesRawData
    {
        [RawData(FieldName = "Product Name")]
        public string RawProductDisplayName { get; set; }

        [RawData(FieldName = "Date")]
        public string RawDate { get; set; }

        [RawData(FieldName = "Price")]
        public string RawPrice { get; set; }

        public DateTime Date => DateParse.ParseDayFirst(RawDate, nameof(RawDate));

        public decimal Price =>
            Decimal.TryParse(RawPrice, out decimal price)
                ? price
                : throw new ApplicationException($"[{RawPrice}] couldn't be parsed as price .");
    }
}