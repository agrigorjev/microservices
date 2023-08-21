using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using com.latencybusters.lbm;
using JetBrains.Annotations;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Client.Bus.Messages.Base;
using Mandara.Common.TaskSchedulers;
using Mandara.Entities.ErrorReporting;
using Ninject;
using Ninject.Extensions.Logging;
using Optional;

namespace Mandara.Business.Bus.Handlers.Base
{
    public class HandlerManager
    {
        private readonly IKernel _kernel;
        private readonly ILogger _log;
        private readonly TaskScheduler _scheduler;
        private readonly ConcurrentDictionary<string, Type> _handlerTypes;

        public HandlerManager()
            : this(IoC.Kernel, IoC.Get<ILoggerFactory>().GetLogger(typeof(BusClient)))
        {

        }

        public HandlerManager([NotNull] IKernel kernel, [NotNull] ILogger log)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            _handlerTypes = new ConcurrentDictionary<string, Type>();
            _scheduler = new LimitedConcurrencyLevelTaskScheduler(20);
        }

        public void RegisterHandler(string topicName, Type handlerType)
        {
            if (string.IsNullOrEmpty(topicName) || null == handlerType)
            {
                return;
            }

            if (!typeof(IHandler).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException("Should inherit from IHandler", nameof(handlerType));
            }

            _handlerTypes.AddOrUpdate(topicName, handlerType, (topic, handler) => handlerType);
        }

        public void HandleMessage(LBMMessage message)
        {
            if (message == null)
            {
                return;
            }

            long receivedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            message.promote();

            Task.Factory.StartNew(
                () => { TryHandleMessage(message, receivedTimestamp); },
                CancellationToken.None,
                TaskCreationOptions.None,
                _scheduler);

        }

        private void TryHandleMessage(LBMMessage message, long receivedEpoch)
        {
            string userName = "Unknown";
            bool isSnapshotMessage = false;

            try
            {
                if (IsAlreadyHandled(message))
                {
                    return;
                }

                string topicName = message.topicName();
                _log.Debug("Handling message on topic {0}",topicName);
                MessageBase irmMessage = GetIrmMessage(message, topicName, userName);

                if (irmMessage.IsDefault())
                {
                    return;
                }

                if (!string.IsNullOrEmpty(irmMessage.UserName))
                {
                    userName = irmMessage.UserName;
                }

                if (irmMessage.Version != InformaticaData.MessageVersion)
                {
                    ReportMessageVersionMismatch(irmMessage, topicName, userName);
                    return;
                }

                isSnapshotMessage = InvokeMessageTypeHandler(message, topicName, receivedEpoch);
            }
            catch (Exception ex)
            {
                ErrorReportingHelper.ReportError(
                    userName,
                    "Handler Manager",
                    ErrorType.Exception,
                    "Handler encountered an error",
                    null,
                    ex,
                    ErrorLevel.Critical);
            }
            finally
            {
                try
                {
                    if (!isSnapshotMessage)
                    {
                        message.Dispose();
                    }

                }
                catch (Exception)
                {
                    // Ignored - this is here for code that hasn't yet been changed to leave the disposing of the
                    // message to this line here.
                }
            }
        }

        private bool IsAlreadyHandled(LBMMessage message)
        {
            return InformaticaHelper.HandleByWatchdog(message);
        }

        private static MessageBase GetIrmMessage(LBMMessage message, string topicName, string userName)
        {
            byte[] bytes = message.data();

            try
            {
                return JsonHelper.Deserialize<MessageBase>(bytes) ?? MessageBase.Default;
            }
            catch (Exception ex)
            {
                string rawMsg = System.Text.Encoding.UTF8.GetString(bytes);

                ErrorReportingHelper.ReportError(
                    userName,
                    "Message Handler",
                    ErrorType.Exception,
                    $"Cannot parse following message [{rawMsg}]. Topic name [{topicName}]",
                    null,
                    ex,
                    ErrorLevel.Critical);

                return MessageBase.Default;
            }
        }

        private static void ReportMessageVersionMismatch(MessageBase irmMessage, string topicName, string userName)
        {
            string errroMsg = string.Format(
                "Received message version [{0}] isn't equal to current message version [{1}]. Topic name [{2}]",
                irmMessage.Version,
                InformaticaData.MessageVersion,
                topicName);

            ErrorReportingHelper.ReportError(
                userName,
                "Message Handler",
                ErrorType.Information,
                errroMsg,
                null,
                null,
                ErrorLevel.Critical);
        }

        private bool InvokeMessageTypeHandler(LBMMessage message, string topicName, long receivedEpoch)
        {
            return _handlerTypes.TryGetValue(topicName, out Type handlerType)
                   && UseKnownHandler(message, topicName, handlerType, receivedEpoch);
        }

        private bool UseKnownHandler(LBMMessage message, string topicName, Type handlerType, long receivedEpoch)
        {
            (_kernel.Get(handlerType) as IHandler)?.Handle(topicName, message, receivedEpoch);
            return IsSnapshotHandler(handlerType);

            bool IsSnapshotHandler(Type handler)
            {
                return handler.IsConstructedGenericType
                       && HasBaseType(handler.GenericTypeArguments[0], typeof(SnapshotMessageBase));
            }

            bool HasBaseType(Type child, Type targetType)
            {
                if (child == null)
                {
                    return false;
                }

                return child.BaseType == targetType || HasBaseType(child.BaseType, targetType);
            }
        }
    }
}