using Mandara.Business.Bus.Messages.Trades;
using Mandara.Entities.Dto;

namespace Mandara.Business.Bus.Messages.EditTrades
{
    public class ProductAliasDto
    {
        public int AliasId { get; set; }
        public string Name { get; set; }
        public ProductDto Product { get; set; }
    }
}