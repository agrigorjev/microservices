using System;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Pnl
{
    public class ChangeSnapshotTimeRequestMessage : MessageBase
    {
        public DateTime SnapshotTime { get; set; }
    }
}