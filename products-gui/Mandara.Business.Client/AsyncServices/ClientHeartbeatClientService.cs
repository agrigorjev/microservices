using Mandara.Business.Bus;
using Mandara.Business.Bus.Commands;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Shared;
using Ninject.Extensions.Logging;
using System;
using System.Configuration;
using Mandara.Business.Bus.Messages;

namespace Mandara.Business.AsyncServices
{
    public class ClientHeartbeatClientService : ClientAsyncService
    {
        private readonly ClientType _clientType;
        private readonly Guid _clientGuid;
        private const int DefaultPeriodSeconds = 10;

        public ClientHeartbeatClientService(
            Guid clientGuid,
            CommandManager commandManager,
            BusClient busClient,
            ILogger log) : base(commandManager, busClient, log)
        {
            _clientGuid = clientGuid;
            _clientType = ClientTypeHelper.DetermineClientType();
            SleepTime = TimeSpan.FromSeconds(HeartbeatPeriod());
        }

        private static int HeartbeatPeriod()
        {
            return int.TryParse(
                ConfigurationManager.AppSettings["ClientHeartbeatPeriod_Seconds"],
                out int clientHeartbeatPeriod)
                ? clientHeartbeatPeriod
                : DefaultPeriodSeconds;
        }

        protected override void DoWork()
        {
            if (BusClient.Instance == null)
            {
                return;
            }

            (new ClientHeartbeatCommand(_clientType, _clientGuid)).Execute();
        }


    }
}
