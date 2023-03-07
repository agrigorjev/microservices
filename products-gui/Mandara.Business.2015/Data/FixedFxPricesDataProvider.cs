using System;
using System.Collections.Generic;
using Mandara.Business.DataInterface;
using Ninject.Extensions.Logging;

namespace Mandara.Business.Data
{
    public class FixedFxPricesDataProvider : FxPricesDataProvider, IFixedFxPricesDataProvider
    {
        private Dictionary<string, decimal> _livePrices;

        public FixedFxPricesDataProvider(ICurrencyProvider currProvider, ILogger log) : base(currProvider, log)
        {
            Reset();
        }

        public override Dictionary<string, decimal> GetLivePrices()
        {
            return _livePrices;
        }

        public void Reset()
        {
            _livePrices = base.GetLivePrices().Clone();
        }
    }
}