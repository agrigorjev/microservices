using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using com.latencybusters.lbm;
using Mandara.Business.Bus.Commands;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.ErrorReporting;
using Ninject.Extensions.Logging;

namespace Mandara.Business.Bus
{
    public class InformaticaResponder
    {
        public Action<byte[]> Respond { get; }
        public Action Dispose { get; }
        public string Target { get; }
        public MessageBase Request { get; }

        public InformaticaResponder(Action<byte[]> respond, Action dispose, string target, MessageBase req)
        {
            Respond = respond;
            Dispose = dispose;
            Target = target;
            Request = req ?? MessageBase.Default;
        }
    }

    public partial class InformaticaHelper
    {
        /// <summary>List of topics whose messages should be dumped</summary>
        public static string[] DumpedTopics
        {
            get
            {
                string value = ConfigurationManager.AppSettings["DumpedTopics"];
                return value != null
                    ? value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    : new string[] { };
            }
        }

        public LBMContext LbmContext { get; protected set; }
        public HandlerManager HandlerManager { get; protected set; }

        protected static readonly ILogger _log = new NLogLogger(typeof(InformaticaHelper));

        protected InformaticaHelper(LBMContext lbmContext, HandlerManager handlerManager)
        {
            LbmContext = lbmContext;
            HandlerManager = handlerManager;
            StartRequestWatchdogCacheCleaningService();
        }

        public virtual SnapshotCommandBase GetSnapshotCommand(string topicName, ref SnapshotMessageBase message)
        {
            return null;
        }

        public virtual SnapshotDeliveryCommand GetSnapshotDeliveryCommand(Guid snapshotId, SnapshotMessageBase message)
        {
            return null;
        }

        public static void SendMessageToSource(LBMSource source, string topicName, byte[] msg)
        {
            if (source == null)
            {
                return;
            }

            _log.Info("Sending message to topic [{0}]", topicName);

            try
            {
                if (!source.isClosed())
                {
                    _log.Trace("Sending {0} bytes to {1}.", msg.Length, topicName);
                    source.send(msg, msg.Length, LBM.SRC_BLOCK);
                    _log.Trace("Send complete for {0} bytes to {1}.", msg.Length, topicName);
                    DumpSentMessage(topicName, msg);
                }
            }
            catch (Exception ex)
            {
                string txt = string.Format("Error sending message to topic [{0}]", topicName);

                ErrorReportingHelper.GlobalQueue.Enqueue(
                    new Error("IRM Server", ErrorType.Exception, txt, null, ex, ErrorLevel.Critical));
            }
        }

        public static void SendRequestToSource(
            LBMSource source,
            string topicName,
            byte[] msg,
            LBMResponseCallback callback)
        {
            SendRequestToSource(source, topicName, msg, callback, null);
        }

        public static void SendRequestToSource(
            LBMSource source,
            string topicName,
            byte[] msg,
            LBMResponseCallback callback,
            object additionalCallbackArgs)
        {
            if (source == null)
            {
                return;
            }

            _log.Info("Sending request to topic [{0}] size [{1}] bytes", topicName, msg.Length);

            try
            {
                if (!source.isClosed())
                {
                    LBMRequest request = new LBMRequest(msg, msg.Length);
                    TempRequests.Add(request);
                    request.addResponseCallback(callback, additionalCallbackArgs);
                    source.send(request, LBM.SRC_BLOCK);
                    DumpSentMessage(topicName, msg);
                }
                else
                {
                    _log.Warn("Cannot send message to {0} source closed", topicName);
                }

                if (_log.IsDebugEnabled)
                {
                    _log.Debug("sent request to topic [{0}] content was [{1}]", topicName, Encoding.UTF8.GetString(msg));
                }
            }
            catch (Exception ex)
            {
                ErrorReportingHelper.GlobalQueue.Enqueue(
                    new Error(
                        "IRM Server",
                        ErrorType.Exception,
                        $"Error sending message to topic [{topicName}]",
                        null,
                        ex,
                        ErrorLevel.Critical));
            }
        }

        public static InformaticaResponder BuildResponder(LBMMessage request)
        {
            return new InformaticaResponder(
                dataToSend => request.respond(dataToSend, dataToSend.Length, LBM.SRC_BLOCK),
                () => InformaticaHelper.DisposeMessage(request),
                request.topicName(),
                JsonHelper.Deserialize<MessageBase>(request.data()));
        }

        public static void RespondToMessage(InformaticaResponder respond, IMessage message)
        {
            byte[] data = message.Serialize();
            //MessageBase requestMessage = JsonHelper.Deserialize<MessageBase>(theMessage.data());

            if (!respond.Request.IsDefault() && (respond.Request.RequestId != Guid.Empty))
            {
                _log.Info("Adding watchdog cache record [{0}]", respond.Request.RequestId);
                _requestWatchdogCache.TryUpdate(respond.Request.RequestId, new RequestWatchdogCacheRecord(data), null);
                // creating watchdog cache record
            }

            RespondToMessage(respond, data);
        }

        //private static void RespondToMessage(LBMMessage theMessage, byte[] data)
        private static void RespondToMessage(InformaticaResponder respond, byte[] data)
        {
            //_log.Info("Responding to message, topic name [{0}]", theMessage.topicName());
            _log.Info("Responding to message, topic name [{0}]", respond.Target);

            try
            {
                //theMessage.respond(data, data.Length, LBM.SRC_BLOCK);
                respond.Respond(data);
            }
            catch (Exception ex)
            {
                string topicName = "cannot retrieve";
                try
                {
                    topicName = respond.Target;
                }
                catch
                {
                }

                string txt = $"Error responding to message, topic name [{topicName}]";

                ErrorReportingHelper.GlobalQueue.Enqueue(
                    new Error("IRM Server", ErrorType.Exception, txt, null, ex, ErrorLevel.Critical));
            }
        }

        public static void DisposeMessage(LBMMessage theMessage)
        {
            string topicName = "cannot retrieve";

            try
            {
                topicName = theMessage.topicName();
            }
            catch
            {
            }

            _log.Info("Disposing message, topic name [{0}]", topicName);

            try
            {
                theMessage.dispose();
            }
            catch (Exception ex)
            {
                string txt = string.Format("Error disposing message, topic name [{0}]", topicName);

                ErrorReportingHelper.GlobalQueue.Enqueue(
                    new Error("IRM Server", ErrorType.Exception, txt, null, ex, ErrorLevel.Normal));
            }
        }

        private int OnMessageReceive(object arg, LBMMessage lbmMessage)
        {
            InformaticaLogger.LogMessage(lbmMessage, _log);
            HandlerManager.HandleMessage(lbmMessage);

            return 0;
        }

        private static void DumpSentMessage(string topicName, byte[] data)
        {
            if (!DumpedTopics.Any(topicName.StartsWith))
            {
                return;
            }

            string dataString = Encoding.UTF8.GetString(data);

            _log.Info("Message sent to topic [{0}]; details: {1}", topicName, dataString);
        }

        public static void UpdateMessageUserNameAndIp(IMessage message)
        {
            if (BusClient.Instance != null)
            {
                UserData userData = BusClient.Instance.GetUserData();

                if (userData != null)
                {
                    string currentUser = userData.UserName;
                    message.UserName = currentUser;
                }
            }

            message.UserIp = LocalIpAddress();
            message.SentAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public static string LocalIpAddress()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress firstOrDefault =
                host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            if (firstOrDefault == null)
            {
                return null;
            }

            return firstOrDefault.ToString();
        }

        public void Close()
        {
            CloseSources();
            CloseReceivers();
            CloseCleanUpService();
        }
    }
}