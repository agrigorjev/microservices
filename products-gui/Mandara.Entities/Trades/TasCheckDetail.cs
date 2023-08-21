using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mandara.Entities.Trades
{
    public class TasCheckDetail
    {
        public string CategoryName { get; set; }
        public decimal ExpectedQuantity { get; set; }
        public decimal RealQuantity { get; set; }

        [JsonIgnore]
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public bool PopupError { get; set; }

        public HashSet<int> PortfolioIds { get; set; }

        public TasCheckDetail()
        {
            PortfolioIds = new HashSet<int>();
        }

        public string GetKey()
        {
            return CategoryName;
        }
    }
}