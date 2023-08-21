using Mandara.Business.Contracts;
using Mandara.Business.Model;
using System.Collections.Generic;

namespace Mandara.Business.Position
{
    public interface IPositionsCollection
    {
        void Add(CalculationDetailModel newPos, IDictionary<string, CalculationDetailModel> positions);

        void TryAdd(
            CalculationDetailModel newPos,
            IrmConcurrentDictionary<string, CalculationDetailModel> positions);

        void Remove(CalculationDetailModel posToRemove, IDictionary<string, CalculationDetailModel> positions);
    }
}
