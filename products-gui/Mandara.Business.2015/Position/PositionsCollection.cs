using System;
using Mandara.Business.Contracts;
using Mandara.Business.Model;
using System.Collections.Generic;

namespace Mandara.Business.Position
{
    public class PositionsCollection : IPositionsCollection
    {
        public void Add(CalculationDetailModel newPos, IDictionary<string, CalculationDetailModel> positions)
        {
            Add(newPos, positions, positions.Add);
        }

        public void Add(
            CalculationDetailModel newPos,
            IDictionary<string, CalculationDetailModel> positions,
            Action<string, CalculationDetailModel> pushPosToCollection)
        {
            if (positions.TryGetValue(newPos.Key, out CalculationDetailModel pos))
            {
                pos.Add(newPos);
            }
            else
            {
                //pos = newPos;
                //pos.Coefficients.Add(newPos.EntityId, 1M);
                pushPosToCollection(newPos.Key, newPos);
            }
        }

        public void TryAdd(
            CalculationDetailModel newPos,
            IrmConcurrentDictionary<string, CalculationDetailModel> positions)
        {
            Add(newPos, positions, (posId, position) => positions.TryAdd(posId, position));
        }

        public void Remove(
            CalculationDetailModel posToRemove,
            IDictionary<string, CalculationDetailModel> positions)
        {
            if (positions.TryGetValue(posToRemove.Key, out CalculationDetailModel pos))
            {
                pos.Remove(posToRemove);
            }
        }
    }
}
