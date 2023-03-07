using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.Currencies
{
    public class CurrenciesResponseMessage : MessageBase
    {
        public IEnumerable<Currency> Currencies { get; set; }
    }
}