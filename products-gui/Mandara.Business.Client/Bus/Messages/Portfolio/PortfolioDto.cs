using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mandara.Business.Bus.Messages.Portfolio
{
    public class PortfolioDto
    {
        public int PortfolioId { get; set; }
        public string Name { get; set; }

        public int? ParentId { get; set; }

        public PortfolioDto ParentPortfolio { get; set; }
        public List<PortfolioDto> Portfolios { get; set; }
        public bool IsArchivedDb { get; set; }
        public bool IsErrorBook { get; set; }
    }
}