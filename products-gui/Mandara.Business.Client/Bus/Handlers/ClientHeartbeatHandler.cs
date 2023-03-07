using System.Collections.Concurrent;
using com.latencybusters.lbm;
using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages;

namespace Mandara.Business.Bus.Handlers
{
    public class ClientHeartbeatHandler : MessageHandler<ClientHeartbeatMessage>
    {
        protected override void Handle(ClientHeartbeatMessage message)
        {
            _clientHeartbeatQueue.Enqueue(message);
        }

        private static ConcurrentQueue<ClientHeartbeatMessage> _clientHeartbeatQueue = new ConcurrentQueue<ClientHeartbeatMessage>();

        /// <summary>
        /// Reads messages from queue or wait if the messages not exist
        /// </summary>
        /// <returns></returns>
        public static bool ReadMessage(out ClientHeartbeatMessage message)
        {
            return _clientHeartbeatQueue.TryDequeue(out message);
        }
    }
}
