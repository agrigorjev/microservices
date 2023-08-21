using System;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.TransferErrors
{
    [Serializable]
    public class TransferErrorsRequestMessage : SnapshotMessageBase
    {
        public DateTime? LastDate { get; set; }
    }
}