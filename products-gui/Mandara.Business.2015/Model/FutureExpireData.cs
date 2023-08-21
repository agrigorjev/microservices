using System;
using Mandara.Entities;

namespace Mandara.Business.Model
{
    public class FutureExpireData
    {
        private readonly TimeSpan _futuresExpireTime;
        private readonly Product _product;
        private DateTime _riskTime;

        public FutureExpireData(Product product, DateTime riskTime)
        {
            _product = product;
            _riskTime = riskTime;
            _futuresExpireTime = product.FuturesExpireTimeToday?.TimeOfDay ?? TimeSpan.MaxValue;
        }

        public bool HasPricingEndTimePassedButProductNotExpired()
        {
            if (!_product.PricingEndTime.HasValue)
            {
                return false;
            }

            return _product.PricingEndTime.Value.TimeOfDay <= _riskTime.TimeOfDay &&
                                           _riskTime.TimeOfDay < _futuresExpireTime;
        }

        public bool HasFutureExpired()
        {
            return _riskTime.TimeOfDay >= _futuresExpireTime;
        }

    }
}
