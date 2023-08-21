using System;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Date.Time;

namespace Mandara.Business.Bus.Messages
{
    public class HeartbeatMessage : MessageBase
    {
        public int LiveTradesLastSequence { get; }
        public int LivePositionsLastSequence { get; }
        public int LiveFxExposureDetailLastSequence { get; }
        public string ServerPrefix { get; set; }

        public static readonly HeartbeatMessage EmptyMessage = new HeartbeatMessage();

        private HeartbeatMessage()
        {
        }

        public HeartbeatMessage(int liveTradeSequence, int livePosSequence, int liveFxExposureSequence)
        {
            LiveTradesLastSequence = liveTradeSequence;
            LivePositionsLastSequence = livePosSequence;
            LiveFxExposureDetailLastSequence = liveFxExposureSequence;
        }
    }

    public class ServerHeartbeat
    {
        public const string NoPrefix = "DefinitelyNotAPrefix";
        public string ServerPrefix { get; } = NoPrefix;
        private const int NoSequence = -1;
        public int LiveTradesLastSequence { get; }
        public int LivePositionsLastSequence { get; }
        public int LiveFxExposureDetailLastSequence { get; }
        public DateTime LastHeartbeatUtc { get; }

        public static readonly ServerHeartbeat Default = new ServerHeartbeat();

        private ServerHeartbeat()
        {
            LiveTradesLastSequence = NoSequence;
            LivePositionsLastSequence = NoSequence;
            LiveFxExposureDetailLastSequence = NoSequence;
            LastHeartbeatUtc = DateTime.MinValue;
        }

        public ServerHeartbeat(HeartbeatMessage sequences)
        {
            ServerPrefix = sequences.ServerPrefix ?? NoPrefix;
            LiveTradesLastSequence = sequences.LiveTradesLastSequence;
            LivePositionsLastSequence = sequences.LivePositionsLastSequence;
            LiveFxExposureDetailLastSequence = sequences.LiveFxExposureDetailLastSequence;
            LastHeartbeatUtc = InternalTime.UtcNow();
        }

        public bool IsDefault()
        {
            return Default == this;
        }
    }
}