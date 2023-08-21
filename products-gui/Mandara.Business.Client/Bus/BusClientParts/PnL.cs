using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mandara.Entities;
using Mandara.Entities.Calculation;

namespace Mandara.Business.Bus.BusClientParts
{
    internal class PnL
    {
        private readonly PnlThrottlingHelper _pnlThrottling;
        private readonly ReaderWriterLockSlim _lockPnl = new ReaderWriterLockSlim();
        private Dictionary<int, Dictionary<string, PnlData>> _pnlDict;

        internal PnL()
        {
            _pnlThrottling = new PnlThrottlingHelper();
        }

        public Dictionary<int, Dictionary<string, PnlData>> PnlDictionary
        {
            get
            {
                try
                {
                    _lockPnl.EnterReadLock();

                    if (_pnlDict != null)
                    {
                        return _pnlDict.ToDictionary(x => x.Key, x => x.Value);
                    }

                    return new Dictionary<int, Dictionary<string, PnlData>>();
                }
                finally
                {
                    _lockPnl.ExitReadLock();
                }
            }
            set
            {
                if (value != null)
                {
                    try
                    {
                        _lockPnl.EnterWriteLock();

                        _pnlDict = value;
                        //Record pnl data to history. For further processing.
                        _pnlThrottling.Add(_pnlDict);
                    }
                    finally
                    {
                        _lockPnl.ExitWriteLock();
                    }
                }
            }
        }

        /// <summary>
        /// Returns strored throttled value for LivePnl value
        /// </summary>
        /// <param name="portfolioId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        internal Money GetLivePnlThrottled(int portfolioId, string currency)
        {
            return _pnlThrottling.GetThrottledValue(portfolioId, currency);
        }
    }
}
