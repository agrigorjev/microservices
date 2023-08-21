using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Mandara.Date.Time;
using Mandara.Entities.Enums;
using Mandara.Entities.ErrorReporting;

namespace Mandara.Entities.FeedImport
{
    [DataContract(IsReference = true)]
    public class FeedImportMessage
    {
        public static FeedImportMessage Create(FeedImportMessageType messageType, FeedImportMessageLevel level, string message, string source)
        {
            FeedImportMessage result = new FeedImportMessage();
            result.Time = SystemTime.Now();
            result.Message = message;
            result.MessageType = messageType;
            result.Level = level;
            result.Source = source;
            return result;
        }

        public static FeedImportMessage CreateMessage(string message, string source)
        {
            return Create(FeedImportMessageType.Message, FeedImportMessageLevel.None, message, source);
        }


        public static FeedImportMessage CreateError(string message, string source)
        {
            return CreateError(message, source, FeedImportMessageLevel.Normal);
        }

        public static FeedImportMessage CreateError (string message, string source, FeedImportMessageLevel level)
        {
            return Create(FeedImportMessageType.Error, level, message, source);
        }
        
        [DataMember]
        public DateTime Time { get; set; }

        [DataMember]
        public FeedImportMessageType  MessageType { get; set; }

        [DataMember]
        public FeedImportMessageLevel Level { get; set; }

        [DataMember]
        public string Source { get; set; }
        
        [DataMember]
        public string Message { get; set; }

        
    }
}
