using Mandara.Business.Audit;
using Mandara.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.Managers
{
    public static class TradeScenarioManager
    {
        public static List<TradeScenario> GetTradeScenarios()
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                return cxt.TradeScenarios.Include("Product").Include("Product.OfficialProduct").ToList();
            }
        }

        private static MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(TradeScenarioManager));
        }

        public static void SaveTradeScenario(TradeScenario tradeScenario, AuditContext auditContext)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                var dbTradeScenario =
                    cxt.TradeScenarios.Include("Product").SingleOrDefault(t => t.TradeScenarioId == tradeScenario.TradeScenarioId);

                if (dbTradeScenario != null)
                {
                    cxt.Entry(dbTradeScenario).CurrentValues.SetValues(tradeScenario);

                    dbTradeScenario.Product =
                        cxt.Products.SingleOrDefault(p => p.ProductId == tradeScenario.Product.ProductId);
                }
                else
                {
                    var product = cxt.Products.SingleOrDefault(p => p.ProductId == tradeScenario.Product.ProductId);
                    tradeScenario.Product = product;
                    cxt.TradeScenarios.Add(tradeScenario);
                }

                cxt.SaveChanges();
            }
        }

        public static void DeleteTradeScenario(TradeScenario tradeScenario, AuditContext auditContext)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                var dbTradeScenario = cxt.TradeScenarios.SingleOrDefault(t => t.TradeScenarioId == tradeScenario.TradeScenarioId);
                if (dbTradeScenario != null)
                {
                    cxt.TradeScenarios.Remove(dbTradeScenario);
                    cxt.SaveChanges();
                }
            }
        }
    }
}
