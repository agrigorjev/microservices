using Mandara.Entities;
using Mandara.Extensions.Option;
using Optional;
using Optional.Unsafe;
using System;
using System.Collections.Generic;
using System.Linq;
using Mandara.Extensions.Collections;

namespace Mandara.Business.Data
{
    public class UnitsProvider : ReplaceableDbContext
    {
        private static Dictionary<int, Unit> _unitById = new Dictionary<int, Unit>();
        private static Dictionary<string, Unit> _unitByName = new Dictionary<string, Unit>();

        public void Update(MandaraEntities dbContext)
        {
            IEnumerable<Unit> loadedUnits = dbContext.Units;

            _unitById = new Dictionary<int, Unit>();
            _unitByName = new Dictionary<string, Unit>();

            loadedUnits.ForEach(
                unit =>
                {
                    _unitById.TryAdd(unit.UnitId, unit);
                    _unitByName.TryAdd(unit.Name, unit);
                });
        }


        public Option<Unit> GetUnit(int id)
        {
            bool success = _unitById.TryGetValue(id, out Unit matchingUnit);

            if (!success)
            {
                Update(_dbContextCreator());

                success = _unitById.TryGetValue(id, out matchingUnit);
            }

            return (success && null != matchingUnit) ? Option.Some(matchingUnit) : Option.None<Unit>();
        }

        [Obsolete]
        public TryGetResult<Unit> TryGetUnit(int id)
        {
            Option<Unit> matchingUnit = GetUnit(id);

            return new TryGetRef<Unit>(matchingUnit.ValueOrDefault(), (curr) => !matchingUnit.HasValue);
        }

        public Option<Unit> GetUnit(int id, MandaraEntities dbContext)
        {
            Option<Unit> matchingUnit = GetUnit(id);

            if (matchingUnit.HasValue)
            {
                return matchingUnit;
            }

            Update(dbContext);
            return GetUnit(id);
        }

        [Obsolete]
        public TryGetResult<Unit> TryGetUnit(int id, MandaraEntities dbContext)
        {
            Option<Unit> matchingUnit = GetUnit(id);

            return new TryGetRef<Unit>(matchingUnit.ValueOrDefault(), (curr) => !matchingUnit.HasValue);
        }

        public Option<Unit> GetUnit(string name)
        {
            bool success = _unitByName.TryGetValue(name, out Unit matchingUnit);

            if (!success)
            {
                Update(_dbContextCreator());

                success = _unitByName.TryGetValue(name, out matchingUnit);
            }

            return (success && null != matchingUnit) ? Option.Some(matchingUnit) : Option.None<Unit>();
        }

        public Option<Unit> GetFirstAvailableUnit()
        {
            return _unitById.Any() ? Option.Some(_unitById.First().Value) : Option.None<Unit>();
        }

        public IEnumerable<Unit> Currencies()
        {
            foreach (Unit curr in _unitById.Values)
            {
                yield return curr;
            }
        }
    }
}
