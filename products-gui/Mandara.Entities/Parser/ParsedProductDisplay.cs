using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mandara.Entities;

namespace Mandara.Entities.Parser
{
    public class ParsedProductDisplay : ParsedProduct
    {
        public ParsedProductDisplay(ParsedProduct product)
            : base()
        {
            ParsedProductId = product.ParsedProductId;
            Date1 = product.Date1;
            DateTypeDb1 = product.DateTypeDb1;
            Date2 = product.Date2;
            DateTypeDb2 = product.DateTypeDb2;
            Bid = product.Bid;
            Ask = product.Ask;
            MandaraPrice = product.MandaraPrice;
            Mid = product.Mid;
            MarkedIncorrect = product.MarkedIncorrect;
            Message = CopyMessage(product.Message);
            Product = product.Product;
        }

        public ParsedProductDisplay(YahooMessage message)
            : base()
        {
            ParsedProductId = Int32.MinValue;
            DateTypeDb1 = 0;
            DateTypeDb2 = 0;
            Message = CopyMessage(message);
        }

        private YahooMessage CopyMessage(YahooMessage message)
        {
            return new YahooMessage
            {
                MessageId = message.MessageId,
                MessageBody = message.MessageBody,
                Received = message.Received,
                GroupName = message.GroupName,
                Broker = message.Broker
            };
        }
    }
}
