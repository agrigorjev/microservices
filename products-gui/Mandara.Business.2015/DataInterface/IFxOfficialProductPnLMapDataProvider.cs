using Mandara.Entities.Entities;
using System.Collections.Generic;

namespace Mandara.Business.DataInterface
{
    public interface IFxOfficialProductPnLMapDataProvider
    {
        List<FxOfficialProductPnLMap> GetFxOfficialProductPnLMaps();
    }
}
