using com.latencybusters.lbm;
using Mandara.Business.AsyncServices.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Client.Bus;
using Mandara.Date.Time;
using Mandara.Entities.ErrorReporting;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mandara.Business.Bus.SnapshotReceivers
{
    public class SnapshotReceiver<T> where T : SnapshotMessageBase
    {
        private const int ComposerMaxWaitMs = 500;

        private readonly LBMContext _lbmContext;
        private readonly LBMSource _lbmSnapshotSource;
        private readonly int _responseTimeout;

        private LBMRequest _lbmSnapshotRequest;
        private LBMReceiver _lbmSnapshotReceiver;

        protected string TopicName { get; set; }

        private T _message;
        private Action<T> _callback;
        private Action _snapshotReceivedCallback;
        private Action _snapshotFailureCallback;
        private RequestWatchdog _requestWatchdog;

        private DateTime _snapshotSent;
        private ILogger _log;

        protected SnapshotReceiver(LBMContext lbmContext, LBMSource lbmSnapshotSource)
        {
            _lbmContext = lbmContext;
            _lbmSnapshotSource = lbmSnapshotSource;
            _log = new NLogLoggerFactory().GetCurrentClassLogger();
        }

        public SnapshotReceiver(LBMContext lbmContext, LBMSource lbmSnapshotSource, string topicName, int responseTimeout)
        {
            _lbmContext = lbmContext;
            _lbmSnapshotSource = lbmSnapshotSource;
            _responseTimeout = responseTimeout;

            TopicName = topicName;
            _log = new NLogLoggerFactory().GetCurrentClassLogger();
        }

        public void GetSnapshot(
            T message,
            Action<T> callback,
            Action snapshotReceivedCallback,
            Action snapshotFailureCallback)
        {
            _log.Debug("GetSnapshot [Topic={0}] [Id={1}]", TopicName, message.SnapshotId);
            _snapshotSent = InternalTime.LocalNow();
            _message = message;
            _callback = callback;
            _snapshotReceivedCallback = snapshotReceivedCallback;
            _snapshotFailureCallback = snapshotFailureCallback;

            SendSnapshotRequest();
        }

        private void SendSnapshotRequest()
        {
            SendSnapshotRequest(
                _message,
                SnapshotMessageType.SnapshotRequest,
                _callback,
                _snapshotReceivedCallback,
                _snapshotFailureCallback);
        }

        private void SendSnapshotRequest<TMessage>(
            TMessage message,
            SnapshotMessageType messageType,
            Action<TMessage> callback,
            Action snapshotReceivedCallback = null,
            Action snapshotFailureCallback = null)
            where TMessage : SnapshotMessageBase
        {
            message.UseGuaranteedDelivery = true;

            message.SnapshotMessageType = messageType;

            if (_log.IsDebugEnabled)
            {
                _log.Debug("SendSnapshotRequest [Topic={0}] [Id={1}] [type = {2}] [requestBody = {3}]",
                    TopicName,
                    message.SnapshotId,
                    messageType,
                    JsonHelper.SerializeToString(message));
            }

            if (_lbmSnapshotRequest != null)
                _lbmSnapshotRequest.close();

            Tuple<object, Action> cbArg = Tuple.Create((object)callback, snapshotReceivedCallback);

            if (_lbmSnapshotReceiver == null)
            {
                _requestWatchdog = new RequestWatchdog(
                    message,
                    ProcessMessage,
                    SendRequestBlocking,
                    cbArg,
                    _responseTimeout,
                    snapshotFailureCallback);

                _requestWatchdog.Start();
            }
            else
            {
                message.RequestId = Guid.Empty;
                SendRequest(cbArg, message, OnSnapshotResponse, false);
            }
        }

        private void SendRequestBlocking(object cbArg, IMessage message, LBMResponseCallback callback)
        {
            SendRequest(cbArg, message, callback, true);
        }

        private void SendRequest(object cbArg, IMessage message, LBMResponseCallback callback, bool canBlock)
        {
            byte[] data = JsonHelper.Serialize(message);
            _lbmSnapshotRequest = new LBMRequest(data, data.Length);
            InformaticaHelper.TempRequests.Add(_lbmSnapshotRequest);
            _lbmSnapshotRequest.addResponseCallback(callback, cbArg);


            if (_lbmSnapshotSource != null)
            {
                if (!_lbmSnapshotSource.isClosed())
                {
                    try
                    {
                        //Try optimisticly no-blocking send
                        _lbmSnapshotSource.send(_lbmSnapshotRequest, LBM.SRC_NONBLOCK);
                    }
                    catch (LBMEWouldBlockException)
                    {
                        //Non blocking send failed we need to send blocking fashion
                        if (canBlock)
                        {
                            _lbmSnapshotSource.send(_lbmSnapshotRequest, LBM.SRC_BLOCK);
                        }
                        else
                        {
                            //In case we come in LBM thread we cannot block send on the same thread
                            //Response offloaded to Task thread pool
                            StartTaskWithErrorHandling(() =>
                            {
                                _lbmSnapshotSource.send(_lbmSnapshotRequest, LBM.SRC_BLOCK);
                            });
                        }
                    }
                }
            }

        }

        private void ProcessMessage(object cbArg, byte[] data)
        {
            try
            {
                T message = UnpackMessage(data);
                Tuple<object, Action> tuple = (Tuple<object, Action>)cbArg;
                Action<T> callback = tuple.Item1 as Action<T>;
                Action snapshotReceivedCallback = tuple.Item2 as Action;
                HandleSnapshotResponse(message, callback, snapshotReceivedCallback);
            }
            catch (Exception e)
            {
                _log.Error(e, "Error processing snapshot result");
            }
        }

        /// <summary>
        /// This method called on Informatica thread , callback will be called on another Thread
        /// </summary>
        /// <param name="cbArg"></param>
        /// <param name="lbmreq"></param>
        /// <param name="lbmmsg"></param>
        /// <returns></returns>
        private int OnSnapshotResponse(object cbArg, LBMRequest lbmreq, LBMMessage lbmmsg)
        {
            InformaticaLogger.LogRequestResponse<T>(
                _log,
                nameof(OnSnapshotResponse),
                lbmreq,
                lbmmsg);
            try
            {
                using (lbmmsg)
                {
                    switch (lbmmsg.type())
                    {
                        case LBM.MSG_RESPONSE:
                        {
                            InformaticaResponse.HandleInformaticaResponse(
                                lbmmsg.data(),
                                (responseData) => ProcessMessage(cbArg, responseData));
                        }
                        break;

                        default:
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                _log.Warn(e, "Error processing response");
            }

            return 0;
        }

        private T UnpackMessage(byte[] bytes)
        {
            return JsonHelper.Deserialize<T>(bytes);
        }

        private readonly BlockingCollection<Tuple<byte[], object>> _rawMessages =
            new BlockingCollection<Tuple<byte[], object>>(new ConcurrentQueue<Tuple<byte[], object>>());
        private readonly Dictionary<int, Tuple<T, Action<T>>> _deserializedMessages = new Dictionary<int, Tuple<T, Action<T>>>();
        private AsyncService _composerService = null;
        private Timer _snapshotEndTimer;
        private string _temporaryTopicName;

        private void HandleSnapshotResponse(T message, Action<T> callback, Action snapshotReceivedCallback = null)
        {
            if (!message.SnapshotId.HasValue)
            {
                return;
            }

            if (message.SnapshotMessageType == SnapshotMessageType.SnapshotRequestResponse)
            {
                _log.Debug(
                    "Starting Composer service [Topic={0}] [Id={1}] [type = {2}]",
                    TopicName,
                    message.SnapshotId,
                    message.SnapshotMessageType);

                if (snapshotReceivedCallback != null)
                {
                    snapshotReceivedCallback();
                }

                ILogger log = new NLogLogger(this.GetType());

                _composerService = AsyncServiceFactory.FromAction(Composer_DoWork, log);
                _composerService.SleepTime = TimeSpan.FromMilliseconds(0);
                _temporaryTopicName = string.Format("{0}/{1}", TopicName, message.SnapshotId.Value);

                LBMTopic temporaryTopic = _lbmContext.lookupTopic(_temporaryTopicName);
                _log.Debug(
                    "Creating temp receiver [Topic={0}] [Id={1}] [type = {2}]",
                    _temporaryTopicName,
                    message.SnapshotId,
                    message.SnapshotMessageType);

                _lbmSnapshotReceiver = _lbmContext.createReceiver(temporaryTopic, TempTopicDataReceived, callback);
                InformaticaHelper.TempReceivers.TryAdd(_lbmSnapshotReceiver, false);
                //Wait 100 ms before we tell the server to start responding
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
                SendSnapshotRequest(message, SnapshotMessageType.SnapshotDataRequest, callback);

                StartNewSnapshotEndTimer(message.SnapshotId.Value, TimeSpan.FromSeconds(60));
                _composerService.Start();
            }

            if (message.SnapshotMessageType == SnapshotMessageType.SnapshotDataRetryResponse)
            {
                StartNewSnapshotEndTimer(message.SnapshotId.Value);
            }
        }

        private void StartNewSnapshotEndTimer(Guid snapshotId, TimeSpan? deafaultDelay = null)
        {
            if (_snapshotEndTimer != null)
                _snapshotEndTimer.Dispose();

            TimeSpan timerDelay = deafaultDelay ?? TimeSpan.FromSeconds(15);
            _snapshotEndTimer = new Timer(SnapshotEndTimerCallback, snapshotId, timerDelay, TimeSpan.FromDays(1));
        }

        private void SnapshotEndTimerCallback(object state)
        {
            Guid snapshotId = (Guid)state;

            _snapshotEndTimer.Dispose();

            _log.Debug("Requesting retry, snapshot id [{0}]", snapshotId);
            RequestRetry(snapshotId);
        }

        private void Composer_DoWork(CancellationToken token)
        {
            try
            {
                Tuple<byte[], object> tuple;
                if (_rawMessages.TryTake(out tuple, ComposerMaxWaitMs, token))
                {
                    T message = UnpackMessage(tuple.Item1);

                    if (message.SnapshotId.HasValue) // otherwise this message doesn't use guaranteed delivery
                    {
                        StartNewSnapshotEndTimer(message.SnapshotId.Value);
                    }
                    else
                    {
                        if (_snapshotEndTimer != null)
                            _snapshotEndTimer.Dispose();
                    }

                    Action<T> callback = tuple.Item2 as Action<T>;
                    _log.Debug("Dequeued message [Topic={0}] [Id={1}] [type = {2}] [sequence = {3}]", TopicName,
                        message.SnapshotId,
                        message.SnapshotMessageType, message.SequenceNumber);
                    HandleSnapshotReceived(message, callback);
                }

                _log.Debug("Composer waiting on {0}", _temporaryTopicName);

            }
            catch (OperationCanceledException)
            {
                _log.Debug("Composer stopped");
            }
        }

        private int TempTopicDataReceived(object cbArg, LBMMessage lbmMessage)
        {
            InformaticaLogger.LogResponse<T>(_log, nameof(TempTopicDataReceived), lbmMessage);
            using (lbmMessage)
            {
                try
                {

                    switch (lbmMessage.type())
                    {
                        case LBM.MSG_DATA:
                        byte[] dataCopied = lbmMessage.data().ToArray();
                        T message = UnpackMessage(dataCopied);
                        _log.Debug("Got raw message [Topic={0}] [Id={1}] [type = {2}] [sequence = {3}]",
                            lbmMessage.topicName(), message.SnapshotId,
                            message.SnapshotMessageType, message.SequenceNumber);
                        _rawMessages.Add(Tuple.Create(dataCopied, cbArg));
                        break;
                        case LBM.MSG_BOS:
                        break;

                        case LBM.MSG_EOS:
                        break;

                        case LBM.MSG_UNRECOVERABLE_LOSS:
                        _log.Warn("TempTopicDataReceived Unrecoverable loss for {0}", _temporaryTopicName);
                        break;

                        case LBM.MSG_UNRECOVERABLE_LOSS_BURST:
                        _log.Warn("TempTopicDataReceived Unrecoverable loss burst for {0}", _temporaryTopicName);
                        break;
                        default:
                        break;
                    }
                }
                catch (Exception e)
                {
                    _log.Warn(e, "Error processing TempTopicDataReceived response");
                }
            }

            return 0;
        }


        public virtual void HandleSnapshotReceived(T message, Action<T> callback)
        {
            switch (message.SnapshotMessageType)
            {
                case SnapshotMessageType.Data:
                _log.Debug("Handling Data message [Topic={0}] [Id={1}] [type = {2}] [sequence = {3}]", TopicName, message.SnapshotId,
        message.SnapshotMessageType, message.SequenceNumber);

                if (!_deserializedMessages.ContainsKey(message.SequenceNumber))
                    _deserializedMessages[message.SequenceNumber] = new Tuple<T, Action<T>>(message, callback);
                break;
                case SnapshotMessageType.EndOfSnapshot:
                _snapshotEndTimer.Dispose();

                _log.Debug("Handling EndOfSnapshot message [Topic={0}] [Id={1}] [type = {2}] [sent sequences = {3}]", TopicName, message.SnapshotId,
        message.SnapshotMessageType, string.Join(",", message.SentSequences ?? new List<int>()));

                CheckIntegrityAndRetry(message);
                break;
                default:
                break;
            }
        }

        private void CheckIntegrityAndRetry(T message)
        {
            List<int> missingSequenceNumbers = GetMissingSequenceNumbers(message.SentSequences);

            _log.Debug("GetMissingSequenceNumbers [Topic={0}] [Id={1}] [type = {2}] [sent sequences = {3}] [missing sequences = {4}]",
                TopicName, message.SnapshotId, message.SnapshotMessageType, string.Join(",", message.SentSequences ?? new List<int>()), string.Join(",", missingSequenceNumbers));

            if (missingSequenceNumbers.Any())
            {
                RequestRetry(message.SnapshotId.Value);
            }
            else
            {
                OnSnapshotEndInternal();
            }
        }

        private void RequestRetry(Guid snapshotId)
        {
            //Debug.WriteLine("Requesting retry for snapshot " + snapshotId);
            var message = new SnapshotServiceMessage()
            {
                SnapshotId = snapshotId,
                ReceivedSequences = ReceivedSequenceNumbers,
                UseGuaranteedDelivery = true,
                UseGzip = true
            };

            _log.Debug("RequestRetry [Topic={0}] [Id={1}] [type = {2}] [Received Sequences = {3}]",
                TopicName, message.SnapshotId, message.SnapshotMessageType, string.Join(",", message.ReceivedSequences));
            SendSnapshotRequest(message, SnapshotMessageType.SnapshotDataRetryRequest, m => { });
        }

        private List<int> ReceivedSequenceNumbers { get { return _deserializedMessages.Keys.ToList(); } }

        private List<int> GetMissingSequenceNumbers(List<int> sentSequences)
        {
            if (sentSequences == null) // guaranteed delivery is not supported on server side for this message 
            {
                return new List<int>();
            }

            return sentSequences.Except(ReceivedSequenceNumbers).ToList();
        }

        private void OnSnapshotEndInternal()
        {
            var timeTook = InternalTime.LocalNow().Subtract(_snapshotSent).TotalSeconds;
            _log.Trace("Snapshot received for topic {0} in {1:F2}s", TopicName, timeTook);
            _log.Debug("SnapshotEndInternal [Topic={0}]", TopicName);

            foreach (var message in _deserializedMessages.OrderBy(r => r.Key))
            {
                message.Value.Item2(message.Value.Item1);
            }

            try
            {
                OnSnapshotEnd();
            }
            finally
            {
                Close();
            }
        }

        protected virtual void OnSnapshotEnd() { }

        public void Close()
        {
            try
            {
                _log.Debug("Closing snapshotReceiver {0}", _temporaryTopicName);
                _composerService?.Stop();

                if (_lbmSnapshotRequest != null)
                    _lbmSnapshotRequest.close();

                if (_lbmSnapshotReceiver != null)
                {
                    bool _;
                    if (InformaticaHelper.TempReceivers.TryRemove(_lbmSnapshotReceiver, out _))
                    {
                        _lbmSnapshotReceiver.close();
                    }
                }
            }
            catch
            {
            }
        }

        protected Task StartTaskWithErrorHandling(Action action)
        {
            return Task.Factory.StartNew(action).ContinueWith(t => ReportError(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }

        private void ReportError(Exception ex)
        {
            ErrorReportingHelper.ReportError(
                "IRM",
                ErrorType.Exception,
                string.Format("Snapshot Receiver encounter an error [Topic={0}]", TopicName),
                null,
                ex,
                ErrorLevel.Critical);
        }
    }
}
