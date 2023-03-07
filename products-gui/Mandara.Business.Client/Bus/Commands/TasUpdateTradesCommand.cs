using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.SnapshotReceivers;
using com.latencybusters.lbm;
using Mandara.Entities.Trades;
using Mandara.Business.Bus.Messages.Base;
using System.Threading.Tasks;
using Mandara.Entities.ErrorReporting;

namespace Mandara.Business.Bus.Commands
{
    public class TasUpdateTradesCommand : BusCommandBase
    {
        private readonly TasPriceMessage _message;
        private readonly Action<MessageStatusCode> _callback;
        private LBMSource _updateSource;
        /// <summary>
        /// Create command to send TAS price updates
        /// </summary>
        /// <param name="lst">Changes in price</param>
        /// <param name="callback">Form method to inform user</param>
        /// <param name="userName">Name of user for audit</param>
        public TasUpdateTradesCommand(List<PriceChanges> lst, Action<MessageStatusCode> callback,string userName)
        {
            _message = new TasPriceMessage();
            _message.SnapshotId = Guid.NewGuid();
            _message.StatusCode = MessageStatusCode.UpdateData;
            _message.changedPrices = lst;
            _callback = callback;
            _message.UserName = userName;
            TopicName = InformaticaHelper.TasTradesUpdateTopicName;
        }
        public override void  Execute()
        {
            _updateSource = BusClient.Instance.InformaticaHelper.GetSource(TopicName);
            SendSnapshotMessage(_message, _message.changedPrices.Count, InformaticaHelper.TasPricesUpdatePackageSize);
            SendMessage(new EndOfSnapshotMessage { SnapshotId = _message.SnapshotId.Value }, _updateSource, TopicName, false);
            StartTaskWithErrorHandling(() => getUpdateResult(_message.SnapshotId.Value,MessageStatusCode.ResultRequest));
        }
        public void getUpdateResult(Guid reqId, MessageStatusCode code)
        {
            switch (code)
            {
                case MessageStatusCode.ResultRequest:
                case MessageStatusCode.UpdateData:
                case MessageStatusCode.UpdateInProgress:
                    BusClient.Instance.CommandManager.AddCommand(new UpdateRequestCommand(
                        reqId
                        , _updateSource
                        , TopicName
                        , getUpdateResult));
                    break;
                case MessageStatusCode.UpdateOK:
                case MessageStatusCode.UpdateFailed:
                    StartTaskWithErrorHandling(() => _callback(code));
                    break;

            }
        }



        private void SendSnapshotMessage(TasPriceMessage message, int numEntities, int packSize)
        {
            List<PriceChanges> entities = message.changedPrices;

            int numPacks = (numEntities / packSize) + 1;

            for (int i = 0; i < numPacks; i++)
            {
                int skip = i * packSize;

                List<PriceChanges> pack = entities.Skip(skip).Take(packSize).ToList();

                if (pack.Count > 0)
                {
                    message.changedPrices = pack;

                    SendMessage(message, _updateSource, TopicName, false);
                }
            }
        }
        private Task StartTaskWithErrorHandling(Action action)
        {
            return Task.Factory.StartNew(action).ContinueWith(t => ReportError(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }
        private void ReportError(Exception ex)
        {
            ErrorReportingHelper.ReportError("IRM", ErrorType.Exception,
                                             string.Format("Update Receiver encounter an error [Topic={0}]", TopicName), null,
                                             ex, ErrorLevel.Critical);
        }
        


      

    }
}


