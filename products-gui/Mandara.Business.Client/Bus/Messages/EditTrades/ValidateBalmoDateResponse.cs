namespace Mandara.Business.Bus.Messages.EditTrades
{
    using Mandara.Business.Bus.Messages.Base;

    public class ValidateBalmoDateResponse : MessageBase
    {
        public bool Validated { get; set; }
    }
}
