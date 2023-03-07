using System;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Positions
{
    [Serializable]
    public class PositionsSnapshotMessageBase : SnapshotMessageBase
    {
        private bool _excludeTasTrades = true;
        public DateTime? DailyDate { get; set; }
        public DateTime? RiskDate { get; set; }

        public bool ExcludeTasTrades
        {
            get { return _excludeTasTrades; }
            set { _excludeTasTrades = value; }
        }
    }
}