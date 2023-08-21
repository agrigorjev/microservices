using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.EditTrades
{
    public class EditTradeParamsMessageDto : MessageBase
    {
        public List<ProductAliasDto> ProductAliases { get; set; }
        public List<SecurityDefinitionDto2> SecurityDefinitions { get; set; }
        public List<CompanyAliasDto> CompanyAliases { get; set; }
    }
}