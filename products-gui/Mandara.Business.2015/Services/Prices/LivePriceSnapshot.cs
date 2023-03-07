using KafkaMessaging.KafkaSchema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Services.Prices
{
    internal class LivePriceSnapshot : JsonObject
    {
        public LivePriceSnapshot()
        {
            Prices = new List<PriceItem>();
        }

        public long Sdate { get; set; }

        public List<PriceItem> Prices { get; set; }

        public override JsonObject FromJson(string json)
        {
            return JsonConvert.DeserializeObject<LivePriceSnapshot>(json);
        }
    }

    internal class PriceItem
    {
        public PriceItem(String column)
        {
            Column = column;
            Prices = new List<double>();
        }

        public String Column { get; private set; }
        public List<double> Prices { get; private set; }
    }
}
