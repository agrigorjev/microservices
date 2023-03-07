namespace Mandara.Business.Bus.Messages.Base
{
    public enum SnapshotMessageType
    {
        Data = 100,
        EndOfSnapshot = 101,
        SnapshotRequest = 102,
        SnapshotRequestResponse = 103,
        SnapshotDataRequest = 104,
        SnapshotDataRequestResponse = 105,
        SnapshotDataRetryRequest = 106,
        SnapshotDataRetryResponse = 107,
    }
}
