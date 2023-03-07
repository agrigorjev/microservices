using Mandara.Business.Contracts;
using Mandara.Entities;
using Mandara.Extensions.Option;
using System;

namespace Mandara.Business.Trades
{
    public class TradeBackingDataFinder
    {
        public static Func<int, TryGetResult<OfficialProduct>> GetOfficialProductForSecurityDef(
            MandaraEntities dbContext,
            ISecurityDefinitionsStorage secDefStore,
            IProductsStorage prodStore)
        {
            return (secDefId) =>
            {
                SecurityDefinition secDef = secDefStore.TryGetSecurityDefinition(secDefId).Value;
                Product product = prodStore.TryGetProduct(secDef.product_id.Value).Value;

                return prodStore.TryGetOfficialProduct(dbContext, product.OfficialProductId);
            };
        }
    }
}
