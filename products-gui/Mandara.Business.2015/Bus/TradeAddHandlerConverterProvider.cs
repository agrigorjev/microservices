using Mandara.Business.Contracts;
using Mandara.Business.Data;
using Mandara.Business.Managers;
using Mandara.Business.Services;
using Mandara.Entities;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.Bus
{
    public class TradeAddHandlerConverterProvider : ITradeAddHandlerConverterProvider
    {
        private IProductsStorage _products;

        public TradeAddHandlerConverterProvider(IProductsStorage products)
        {
            _products = products;
        }

        public TradeAddHandlerConverterProvider() : this(new ProductsStorage(new CurrencyProvider()))
        {
            _products.Update();
        }

        public virtual SecurityDefinition GetFxTradesSecurityDefinition()
        {
            using (MandaraEntities context = new MandaraEntities(
                MandaraEntities.DefaultConnStrName,
                nameof(TradeAddHandlerConverterProvider)))
            {
                return context.SecurityDefinitions
                    .FirstOrDefault(it => it.SecurityDefinitionId == SecurityDefinitionsManager.FxSecDefId);
            }
        }

        public virtual TryGetResult<OfficialProduct> GetValidOfficialProduct(int officialProductId, DateTime targetDateTime)
        {
            TryGetResult<OfficialProduct> officialProduct = _products.TryGetOfficialProduct(officialProductId);

            if (officialProduct.HasValue && HasValidProducts(officialProduct.Value, targetDateTime))
            {
                return officialProduct;
            }

            return new TryGetRef<OfficialProduct>();
        }

        private bool HasValidProducts(OfficialProduct offProd, DateTime targetDateTime)
        {
            return
                offProd.Products.Any(
                    product =>
                        (product.ValidFrom == null || product.ValidFrom <= targetDateTime)
                        && (product.ValidTo == null || product.ValidTo >= targetDateTime));
        }


        public virtual List<OfficialProduct> GetOfficialProducts()
        {
            return _products.GetOfficialProducts().ToList();
        }
    }
}
