using Mandara.Entities;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Bus
{
    public interface ITradeAddHandlerConverterProvider
    {
        SecurityDefinition GetFxTradesSecurityDefinition();

        TryGetResult<OfficialProduct> GetValidOfficialProduct(int officialProductId, DateTime targetDateTime);

        List<OfficialProduct> GetOfficialProducts();
    }
}
