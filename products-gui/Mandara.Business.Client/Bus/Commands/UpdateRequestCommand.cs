using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages.Base;
using com.latencybusters.lbm;
using System.Threading.Tasks;
using Mandara.Entities.ErrorReporting;

namespace Mandara.Business.Bus.Commands
{
    using Mandara.Business.Bus.Messages;
    using Mandara.Entities.Exceptions;

    class UpdateRequestCommand : BusCommandBase
    {
        private readonly Action<Guid, MessageStatusCode> _callback;
        private readonly Action<Guid, MessageStatusCode, List<Error>> _callback2;
        private readonly Guid _resultID;
        private readonly LBMSource _lbmSnapshotSource;


        public UpdateRequestCommand(Guid id, LBMSource lbmSnapshotSource, string topicName, Action<Guid, MessageStatusCode> callback)
        {
            TopicName = topicName;
            _lbmSnapshotSource = lbmSnapshotSource;
            _resultID = id;
            _callback = callback;
        }
        public UpdateRequestCommand(Guid id, LBMSource lbmSnapshotSource, string topicName, Action<Guid, MessageStatusCode, List<Error>> callback)
        {
            TopicName = topicName;
            _lbmSnapshotSource = lbmSnapshotSource;
            _resultID = id;
            _callback2 = callback;
            _callback = null;
        }
        public override void Execute()
        {
            SendRequest(new SnapshotServiceMessage { SnapshotId = _resultID, StatusCode = MessageStatusCode.ResultRequest });
        }
        private void SendRequest(SnapshotMessageBase message)
        {
            InformaticaHelper.UpdateMessageUserNameAndIp(message);
            byte[] msg = JsonHelper.Serialize(message);

            if (_callback != null)
            {
                InformaticaHelper.SendRequestToSource(_lbmSnapshotSource, TopicName, msg, OnSnapshotResponse, _callback);
            }
            else if (_callback2 != null)
            {
                InformaticaHelper.SendRequestToSource(_lbmSnapshotSource, TopicName, msg, OnSnapshotResponse2, _callback2);
            }
        }

        private int OnSnapshotResponse(object cbArg, LBMRequest lbmreq, LBMMessage lbmmsg)
        {
            switch (lbmmsg.type())
            {
                case LBM.MSG_RESPONSE:
                    MessageBase message = JsonHelper.Deserialize<MessageBase>(lbmmsg.data());
                    StartTaskWithErrorHandling(() => _callback(_resultID, message.StatusCode));
                    break;

                default:
                    break;
            }
            lbmmsg.dispose();
            return 0;
        }
        private int OnSnapshotResponse2(object cbArg, LBMRequest lbmreq, LBMMessage lbmmsg)
        {
            switch (lbmmsg.type())
            {
                case LBM.MSG_RESPONSE:
                    CSVResultMessage message = JsonHelper.Deserialize<CSVResultMessage>(lbmmsg.data());
                    StartTaskWithErrorHandling(() => _callback2(_resultID, message.StatusCode, message.Warnings));
                    break;

                default:
                    break;
            }
            lbmmsg.dispose();
            return 0;
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
