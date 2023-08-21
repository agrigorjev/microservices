using System;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Commands
{
    public class MatchingDummiesSnapshotCommand : BusCommandBase
    {
        private MatchingDummiesMessageArgs _args;
        Action<MatchingDummiesSnapshotMessage> _callback;

        public MatchingDummiesSnapshotCommand(MatchingDummiesMessageArgs args, Action<MatchingDummiesSnapshotMessage> callback)
        {
            _args = args;
            _callback = callback;
            TopicName = InformaticaHelper.MatchSnapshotTopicName;
        }

        private void OnResponse(byte[] responseData)
        {
            //TODO : add error handling, verion check
            if (_callback != null && responseData != null && responseData.Length > 0)
                _callback(MessageBase.Deserialize<MatchingDummiesSnapshotMessage>(responseData));
        }
        #region Overrides of BusCommandBase

        public override void Execute()
        {
            SendRequest(_args, OnResponse);
        }

        #endregion
    }

    public class MatchingDummiesWrongCommand : BusCommandBase
    {
        private MatchingDummiesWrongMessageArgs _args;

        public MatchingDummiesWrongCommand(MatchingDummiesWrongMessageArgs args)
        {
            _args = args;
            TopicName = InformaticaHelper.WrongMatchTopicName;
        }

        #region Overrides of BusCommandBase

        public override void Execute()
        {
            SendMessage(_args);
        }

        #endregion
    }

    public class MatchingDummiesUpdateCommand : BusCommandBase
    {
        private MatchingDummiesSnapshotMessage _matchUpdate;

        public MatchingDummiesUpdateCommand(MatchingDummiesSnapshotMessage matchUpdate)
        {
            _matchUpdate = matchUpdate;
            TopicName = InformaticaHelper.MatchUpdateTopicName;
        }

        #region Overrides of BusCommandBase

        public override void Execute()
        {
            SendMessage(_matchUpdate);
        }

        #endregion
    }

}