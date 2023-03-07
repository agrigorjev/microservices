namespace Mandara.Business.Bus.Messages
{
    using Mandara.Business.Bus.Messages.Base;

    public sealed class EndOfSnapshotMessage : SnapshotMessageBase
    {
        public EndOfSnapshotMessage()
        {
            SnapshotMessageType = SnapshotMessageType.EndOfSnapshot;
        }
    }
}
