using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.TransferErrors
{
    [Serializable]
    public class TransferErrorsResponseMessage : SnapshotMessageBase
    {
        public List<TradeTransferErrorDto> TransferErrors { get; set; }

        public override void OnErrorSet()
        {
            TransferErrors.Clear();
        }
    }
}