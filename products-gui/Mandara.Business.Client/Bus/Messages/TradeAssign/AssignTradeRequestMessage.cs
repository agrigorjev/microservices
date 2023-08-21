using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Json;
using Newtonsoft.Json;

namespace Mandara.Business.Bus.Messages.TradeAssign
{
    public class AssignTradeRequestMessage : MessageBase
    {
        public int ToPortfolioId { get; set; }
        public string ToPortfolioName { get; set; }
        [JsonProperty(ItemConverterType = typeof(ExecIdDatesKeyValuePairConverter))]
        public List<KeyValuePair<string, Tuple<List<DateTime?>, List<string>>>> ExecIdsOfTradesToAssign { get; set; }

        public AssignTradeRequestMessage()
        {
        }

        public AssignTradeRequestMessage(int toPortfolioId, string toPortfolioName, Dictionary<string, Tuple<List<DateTime?>, List<string>>> execIdsOfTradesToAssign)
        {
            ToPortfolioId = toPortfolioId;
            ToPortfolioName = toPortfolioName;

            if (execIdsOfTradesToAssign != null) 
                ExecIdsOfTradesToAssign = execIdsOfTradesToAssign.ToList();
        }
    }
}
