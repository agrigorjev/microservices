using System;
using Mandara.Entities.Enums;

namespace Mandara.Business.Bus.Messages.TransferErrors
{
    [Serializable]
    public class TradeTransferErrorDto
    {
        public TransferErrorType ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public string EntityId { get; set; }
        public DateTime ErrorDate { get; set; }
        public string Details { get; set; }
        public TransferEntityType EntityType { get; set; }
    }
}