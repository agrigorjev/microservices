using System;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Commands
{
    public class PnlHistoricalDateCommand : BusCommandBase
    {
        private PnlHistoricalDateMessageArgs _requestArgs;
        private Action<PnlHistoricalDateMessage> _callback;

        public PnlHistoricalDateCommand(PnlHistoricalDateMessageArgs args, Action<PnlHistoricalDateMessage> callback)
        {
            _requestArgs = args;
            _callback = callback;
            TopicName = InformaticaHelper.PnlHistoricalDateTopicName;
        }

        private void OnResponse(byte[] responseData)
        {
            //TODO : add error handling, verion check
            if (_callback != null && responseData!=null && responseData.Length>0)
                _callback(MessageBase.Deserialize<PnlHistoricalDateMessage>(responseData));
        }
        #region Overrides of BusCommandBase

        public override void Execute()
        {
            SendRequest(_requestArgs, OnResponse);
        }

        #endregion
    }

    public class PnlHistoricalInformationCommand : BusCommandBase
    {
        private PnlHistoricalInformationMessageArgs _requestArgs;
        private Action<PnlHistoricalInformationMessage> _callback;

        public PnlHistoricalInformationCommand(PnlHistoricalInformationMessageArgs args, Action<PnlHistoricalInformationMessage> callback)
        {
            _requestArgs = args;
            _callback = callback;
            TopicName = InformaticaHelper.PnlHistoricalInformationTopicName;
        }

        private void OnResponse(byte[] responseData)
        {
            //TODO : add error handling, verion check
            if (_callback != null && responseData!=null && responseData.Length>0)
                _callback(MessageBase.Deserialize<PnlHistoricalInformationMessage>(responseData));
        }
        #region Overrides of BusCommandBase

        public override void Execute()
        {
            SendRequest(_requestArgs, OnResponse);
        }

        #endregion
    }

    public class PnlHistoricalSaveCommand : BusCommandBase
    {
        private PnlHistoricalSaveMessageArgs _requestArgs;
        private Action<PnlHistoricalSaveMessage> _callback;

        public PnlHistoricalSaveCommand(PnlHistoricalSaveMessageArgs args, Action<PnlHistoricalSaveMessage> callback)
        {
            _requestArgs = args;
            _callback = callback;
            TopicName = InformaticaHelper.PnlHistoricalSaveTopicName;
        }

        private void OnResponse(byte[] responseData)
        {
            //TODO : add error handling, verion check
            if (_callback != null && responseData!=null && responseData.Length>0)
                _callback(MessageBase.Deserialize<PnlHistoricalSaveMessage>(responseData));
        }
        #region Overrides of BusCommandBase

        public override void Execute()
        {
            SendRequest(_requestArgs, OnResponse);
        }

        #endregion
    }
}