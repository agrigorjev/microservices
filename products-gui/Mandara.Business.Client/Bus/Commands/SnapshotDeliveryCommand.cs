using System;
using System.Linq;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Commands
{
    public class SnapshotDeliveryCommand : SnapshotCommandBase
    {
        public static SnapshotDeliveryCommand FromSnapshotCommand(SnapshotCommandBase command)
        {
            var result = new SnapshotDeliveryCommand()
                             {
                                 LbmTemporarySource = command.LbmTemporarySource,
                                 TopicName = command.TopicName,
                                 Message = command.Message,
                                 DeliveryContext = command.DeliveryContext,
                                 ResponseTimeout = command.ResponseTimeout,
                                 SupportsGuaranteedDelivery = command.SupportsGuaranteedDelivery
                             };

            return result;
        }

        public override void DoExecute()
        {
            SnapshotMessageBase requestMessage = Message as SnapshotMessageBase;

//            // todo: test code
//            Random random = new Random();

            var sequenceNumbers = DeliveryContext.SnapshotData.SequenceNumbers.ToList();

            foreach (var sequenceNumber in sequenceNumbers)
            {
                var message = DeliveryContext.SnapshotData[sequenceNumber];

                if (message != null)
                {
//                    // todo: test code
//                    if (random.NextDouble() > 0.8)
                    {
                        Log.Debug("Sending message with sequence " + message.SequenceNumber);
                        SendMessage(message, requestMessage.UseGzip);
                    }
                }
            }

//            // todo: test code
//            if (random.NextDouble() > 0.8)
            {
                SendEndOfSnapshot();
            }
        }
    }
}
