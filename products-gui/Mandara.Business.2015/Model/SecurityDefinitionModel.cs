using System.Collections.Generic;

namespace Mandara.Business.Model
{
    public class SecurityDefinitionModel
    {
        public int SecurityDefinitionId { get; set; }
        public int? ProductId { get; set; }
        public List<PrecalcDetailModel> PrecalcDetails { get; set; }
    }
}