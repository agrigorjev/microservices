using System;

namespace Mandara.Business.Model
{
    public class PrecalcDetailModel
    {
        public DateTime Month { get; set; }
        public int ProductId { get; set; }
        public string DaysSerialized { get; set; }
        public DateTime MinDay { get; set; }
        public DateTime MaxDay { get; set; }
    }
}