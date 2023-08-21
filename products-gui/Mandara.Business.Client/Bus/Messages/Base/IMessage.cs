namespace Mandara.Business.Bus.Messages.Base
{
    using System;

    public interface IMessage
    {
        string Version { get; set; }
        MessageStatusCode StatusCode { get; set; }
        Guid RequestId { get; set; }
        string UserName { get; set; }
        string UserIp { get; set; }
        int SequenceNumber { get; set; }

        /// <summary>
        /// UTC timestamp of when this has been sent at
        /// </summary>
        long SentAt { get; set; }
        long ReceivedAt { get; set; }

        byte[] Serialize();
    }
}