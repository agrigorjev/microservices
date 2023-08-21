using Mandara.Entities;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Mandara.Business
{
    public class TradeTemplatesManager
    {
        public List<TradeTemplate> GetTradeTemplates(Portfolio portfolio = null)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                DbQuery<TradeTemplate> objectQuery =
                    cxt.TradeTemplates.Include("Exchange").Include("OfficialProduct").Include("Portfolio").Include("Unit");

                if (portfolio != null)
                {
                    objectQuery = (DbQuery<TradeTemplate>)objectQuery.Where(x => x.Portfolio.PortfolioId == portfolio.PortfolioId);
                }

                return objectQuery
                          .OrderBy(x => x.TemplateName)
                          .ToList();
            }
        }

        private static MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(TradeTemplatesManager));
        }

        public void Save(TradeTemplate tradeTemplate)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                TradeTemplate templateDb = cxt.TradeTemplates.FirstOrDefault(x => x.TradeTemplateId == tradeTemplate.TradeTemplateId) ?? new TradeTemplate();

                templateDb.TemplateName = tradeTemplate.TemplateName;
                templateDb.Portfolio =
                    cxt.Portfolios.FirstOrDefault(x => x.PortfolioId == tradeTemplate.Portfolio.PortfolioId);
                templateDb.OfficialProduct =
                    cxt.OfficialProducts.FirstOrDefault(
                        x => x.OfficialProductId == tradeTemplate.OfficialProduct.OfficialProductId);
                templateDb.Exchange =
                    cxt.Exchanges.FirstOrDefault(x => x.ExchangeId == tradeTemplate.Exchange.ExchangeId);
                templateDb.Volume = tradeTemplate.Volume;
                templateDb.Unit = cxt.Units.FirstOrDefault(x => x.UnitId == tradeTemplate.Unit.UnitId);

                if (tradeTemplate.TradeTemplateId == 0)
                {
                    cxt.TradeTemplates.Add(templateDb);
                }

                cxt.SaveChanges();
            }
        }

        public void Delete(TradeTemplate tradeTemplate)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                TradeTemplate templateDb = cxt.TradeTemplates.FirstOrDefault(x => x.TradeTemplateId == tradeTemplate.TradeTemplateId);

                if (templateDb == null)
                    return;

                cxt.TradeTemplates.Remove(templateDb);

                cxt.SaveChanges();
            }
        }
    }
}