using System;
using System.ComponentModel;
using AutoMapper;
using Mandara.Business.Client.Bus.Messages.Base;
using Newtonsoft.Json;

namespace Mandara.Business.Bus.Messages.Base
{
    [Serializable]
    public class MessageBase : IMessage
    {
        public virtual string Version { get; set; }
        public virtual MessageStatusCode StatusCode { get; set; }
        public virtual string UserName { get; set; }
        public virtual string UserIp { get; set; }
        public virtual int SequenceNumber { get; set; }
        public virtual bool UseGzip { get; set; } = true;

        /// <summary>UTC timestamp of when this message sent at</summary>
        public virtual long SentAt { get; set; }

        /// <summary>UTC timestamp of when this message received at</summary>
        public virtual long ReceivedAt { get; set; }

        public const string DefaultVersion = "NoVersion";
        public const MessageStatusCode DefaultStatus = MessageStatusCode.Ok;
        public const string DefaultUser = "NoUser";
        public const int DefaultSequence = -1;

        [ReadOnly(true)]
        public static readonly MessageBase Default = new MessageBase()
        {
            Version = DefaultVersion,
            StatusCode = DefaultStatus,
            UserName = DefaultUser,
            UserIp = DefaultUser,
            SequenceNumber = DefaultSequence,
        };

        [ReadOnly(true)]
        public bool IsDefault()
        {
            return DefaultVersion == Version
                   && DefaultStatus == StatusCode
                   && DefaultUser == UserName
                   && DefaultUser == UserIp
                   && DefaultSequence == SequenceNumber;
        }

        /// <summary>Service field for RequestWatchdog</summary>
        public Guid RequestId { get; set; }

        public MessageBase()
        {
            Version = InformaticaData.MessageVersion;
        }

        public virtual byte[] Serialize()
        {
            return JsonHelper.Serialize(this);
        }

        public static T Deserialize<T>(byte[] buf) where T : class, IMessage, new()
        {
            return JsonHelper.Deserialize<T>(buf);
        }

        public bool HasTimeStamp()
        {
            return SentAt != 0 && ReceivedAt != 0;
        }

        [JsonIgnore]
        [IgnoreMap]
        public TimeSpan ReceivedTimeDelay => TimeSpan.FromMilliseconds(ReceivedAt - SentAt);
    }

    [Serializable]
    public abstract class MessageWithErrorBase : MessageBase
    {
        private string _errorMessage;

        protected MessageWithErrorBase()
        {
            Success = true;
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;

                Success = string.IsNullOrEmpty(value);

                if (!Success)
                {
                    OnErrorSet();
                }
            }
        }

        public bool Success { get; protected set; }

        public virtual void SetError(string errorMessage)
        {
            HandleInvalidErrorMessage(errorMessage);
            OnErrorSet();
            ErrorMessage = errorMessage;
            Success = false;
        }

        protected static void HandleInvalidErrorMessage(string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                throw new ArgumentException(
                    "Error message must be provided and cannot be null or empty",
                    nameof(errorMessage));
            }
        }

        public abstract void OnErrorSet();
    }
}