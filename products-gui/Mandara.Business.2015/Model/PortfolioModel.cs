using System.Collections.Generic;

namespace Mandara.Business.Model
{
    public class PortfolioModel
    {
        public int PortfolioId { get; set; }
        public string Name { get; set; }
        public PortfolioModel ParentPortfolio { get; set; }
        public List<PortfolioModel> Portfolios { get; set; }
        public bool IsArchivedDb { get; set; }
        public bool IsErrorBook { get; set; }
    }
}