using Mandara.Business.Bus.Messages.Base;
using Mandara.Date.Time;
using System;

namespace Mandara.Business.Model
{
    public class MessageCache
    {
        public MessageBase Message { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public MessageCache(MessageBase message)
        {
            Message = message;
            CreatedAt = SystemTime.Now();
        }
    }
}