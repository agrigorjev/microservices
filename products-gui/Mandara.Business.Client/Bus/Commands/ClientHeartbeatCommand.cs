using System;
using System.Linq;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages;

namespace Mandara.Business.Bus.Commands
{
    public class ClientHeartbeatCommand : BusCommandBase
    {
        private readonly ClientType _clientType;
        private readonly Guid _clientConnectionId;

        public ClientHeartbeatCommand(ClientType clientType, Guid clientConnectionId)
        {
            _clientType = clientType;
            _clientConnectionId = clientConnectionId;
            TopicName = InformaticaHelper.ClientHeartbeatTopicName;
        }

        public override void Execute()
        {
            ClientHeartbeatMessage message = new ClientHeartbeatMessage
            {
                ClientGuid = _clientConnectionId,
                ClientType = _clientType,
                ServerPrefix =
                    InformaticaHelper.ServerPrefixes.Any() ? InformaticaHelper.ServerPrefixes[0] : String.Empty,
                LastHeartbeatUtc = BusClient.Instance.LastHeartbeat != null
                    ? BusClient.Instance.LastHeartbeat.LastHeartbeatUtc
                    : DateTime.MinValue
            };

            InformaticaHelper.UpdateMessageUserNameAndIp(message);

            switch (_clientType)
            {
                case ClientType.AlertService:
                {
                    message.UserName = "Alert Service";
                }
                break;

                case ClientType.ReportingService:
                {
                    message.UserName = "Reporting Service";
                }
                break;

                case ClientType.IrmClient:
                {
                    // TODO: Does anything need to be done here?
                }
                break;

                default:
                {
                    throw new ArgumentOutOfRangeException($"{_clientType} is not a recognised {nameof(ClientType)}");
                }
            }

            message.UserName = message.UserName ?? "Logging in...";

            SendMessage(message);
        }
    }
}