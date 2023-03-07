namespace Mandara.Business.Bus.Messages.EditTrades
{
    using System;
    using Mandara.Business.Bus.Messages.Base;

    public class ValidateBalmoDateRequest : MessageBase
    {
        public DateTime BalmoDate { get; set; }
        public int ProductId { get; set; }
    }
}
