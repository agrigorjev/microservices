using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Pnl
{
    public class ChangeSnapshotTimeResponseMessage : MessageBase
    {
        public string ErrorMessage { get; set; }
    }
}