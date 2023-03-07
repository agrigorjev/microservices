using System;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages;

namespace Mandara.Business.Bus.Commands
{
    /// <summary>
    /// Request to get cumulative Pnl from server.
    /// </summary>
    public class RequestCumulativeCommand : BusCommandBase
    {
        private PnlCumulativeArgs _requestArgs;
        private Action<byte[]> _callback;

        public RequestCumulativeCommand(PnlCumulativeArgs args, Action<byte[]> callback)
        {
            _requestArgs = args;
            _callback = callback;

            TopicName = InformaticaHelper.PnlCumulativeSnapshotTopicName;
        }

        //TODO: VV: Set TopicName like this:
        public virtual string Topic
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        private void OnResponse(byte[] responseData)
        {
            //TODO : add error handling, verion check
            if (_callback != null)
                _callback(responseData);
        }

        #region Overrides of BusCommandBase

        public override void Execute()
        {
            SendRequest(_requestArgs, OnResponse);
        }

        #endregion
    }
}