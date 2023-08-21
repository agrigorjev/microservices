using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.EditTrades
{
    public class EditTradeParamsMessage : MessageBase
    {
        public List<ProductAlias> ProductAliases { get; set; }
        public List<SecurityDefinition> SecurityDefinitions { get; set; }
        public List<CompanyAlias> CompanyAliases { get; set; }
    }
}