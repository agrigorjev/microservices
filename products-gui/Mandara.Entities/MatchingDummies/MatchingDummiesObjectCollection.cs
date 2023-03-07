using System.Collections.Generic;
using System.Linq;

namespace Mandara.Entities.MatchingDummies
{
    public class MatchingDummiesObjectCollection:List<MatchingDummiesObject>
    {

        public bool ContainTraidId(int traidId)
        {
            return this.FirstOrDefault(fv => fv.TradeId == traidId) != null;
        }

        public new MatchingDummiesObject this[int traidId]
        {
            get { return this.FirstOrDefault(fv => fv.TradeId == traidId); }
        }
    }
}