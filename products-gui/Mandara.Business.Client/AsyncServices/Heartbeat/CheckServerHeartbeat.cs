using System;
using System.Collections.Generic;
using System.Linq;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Date.Time;
using Mandara.Extensions.Option;

namespace Mandara.Business.Client.AsyncServices.Heartbeat
{
    public class CheckServerHeartbeat
    {
        public static bool IsHeartbeatOk(DateTime lastBeatAtUtc, TimeSpan heartbeatPeriodSeconds)
        {
            return InternalTime.UtcNow() - lastBeatAtUtc <= heartbeatPeriodSeconds;
        }

        public static bool IsHeartbeatLost(DateTime lastNoHeartbeatAtUtc, TimeSpan secondsUntilHeartbeatIsLost)
        {
            return InternalTime.UtcNow() - lastNoHeartbeatAtUtc > secondsUntilHeartbeatIsLost;
        }

        public static List<int> SkippedSequenceNumbers<T>(SequenceQueue<T> sequence, int lastKnownSequenceNumber)
            where T : class, IMessage
        {
            TryGetVal<int> lastSequence = sequence.LastDequeuedSequence;
            List<int> existingSequences = sequence.ExistingSequences;

            return lastSequence.HasValue
                ? SkippedSequences<T>(lastKnownSequenceNumber, lastSequence.Value).Except(existingSequences).ToList()
                : new List<int>();
        }

        private static IEnumerable<int> SkippedSequences<T>(int lastKnownSequenceNumber, int lastSequence)
            where T : class, IMessage
        {
            int firstSkippedSequence = lastSequence + 1;

            return Enumerable.Range(firstSkippedSequence, lastKnownSequenceNumber - firstSkippedSequence).ToHashSet();
        }
    }
}