using System;
using System.Collections.Concurrent;
using com.latencybusters.lbm;
using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Date.Time;

namespace Mandara.Business.Bus.Handlers
{
    public class ServerHeartbeatHandler : MessageHandler<HeartbeatMessage>
    {
        protected override void Handle(HeartbeatMessage heartbeatMessage)
        {
            _heartbeatQueue.Enqueue(new ServerHeartbeat(heartbeatMessage));
        }

        private static readonly ConcurrentQueue<ServerHeartbeat> _heartbeatQueue =
            new ConcurrentQueue<ServerHeartbeat>();


        /// <summary>
        /// Reads messages from queue or wait if the messages not exist
        /// </summary>
        /// <returns></returns>
        public static bool ReadMessage(out ServerHeartbeat heartbeat)
        {
            return _heartbeatQueue.TryDequeue(out heartbeat);
        }

    }
}
