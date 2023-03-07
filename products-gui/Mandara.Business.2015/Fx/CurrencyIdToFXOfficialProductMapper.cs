using Mandara.Entities.Entities;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.Fx
{
    public class CurrencyIdToFXOfficialProductMapper
    {
        private Dictionary<int, FxOfficialProductPnLMap> _currencyIdMaps;
        private ILogger _log;

        public CurrencyIdToFXOfficialProductMapper(ILogger log)
        {
            _currencyIdMaps = new Dictionary<int, FxOfficialProductPnLMap>();
            _log = log;
        }

        public void ResetData(List<FxOfficialProductPnLMap> currencyToFxOfficialProductPnLMaps)
        {
            List<FxOfficialProductPnLMap> tempMaps = currencyToFxOfficialProductPnLMaps
                                                     ?? new List<FxOfficialProductPnLMap>();
            _currencyIdMaps = tempMaps.ToDictionary(
                currencyIdMap => currencyIdMap.CurrencyId,
                currencyIdMap => currencyIdMap);

        }

        public FxOfficialProductPnLMap GetOfficialProductAndHolidayCalendarIDsForCurrency(int currencyId)
        {
            FxOfficialProductPnLMap currencyIdMap;

            if (_currencyIdMaps.TryGetValue(currencyId, out currencyIdMap))
            {
                return currencyIdMap.CloneInstance();
            }

            _log.Warn(
                String.Format(
                    "Could not find an official product ID or holiday calendar for currency ID {0}",
                    currencyId));
            return FxOfficialProductPnLMap.DefaultFxOfficialProductPnLMap(currencyId);
        }

        public bool HasMappingData()
        {
            return _currencyIdMaps.Any();
        }
    }
}
