using Mandara.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandara.Business.Managers
{
    public interface IPerformanceLogManager
    {
        void SaveTradesStats(
            List<int> tradeIds, 
            Action<PerformanceLogMessage, DateTime> setPerformanceLogMessageAction);
    }
}
