using Mandara.Entities;
using System;

namespace Mandara.Business.DataInterface
{
    public interface IFxTradesCache : IEntityCache<FxTrade, Int32>, IFxTradesDataProvider
    {
    }
}
