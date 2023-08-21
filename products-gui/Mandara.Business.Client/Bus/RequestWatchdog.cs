using System;
using System.Threading.Tasks;
using Mandara.Business.Client.Bus;
using Mandara.Date.Time;
using Ninject.Extensions.Logging;

namespace Mandara.Business.Bus
{
    using com.latencybusters.lbm;
    using Mandara.Business.Bus.Messages.Base;

    /// <summary>
    ///     RequestWatchdog is a class that wraps LBMRequest with retry logic. It sends request. If response is not
    ///     received during ResponseTimeout, it retries request MaxRetryAttempts of times. If a response is finally
    ///     received, RequestWatchdog calls ResponseCallback. Else it calls CallbackOnFailure.
    /// </summary>
    internal class RequestWatchdog
    {
        private readonly IMessage _message;
        private readonly Action<object, byte[]> _responseCallback;
        private readonly Action<object, IMessage, LBMResponseCallback> _request;
        private readonly object _additionalCallbackArgs;
        private readonly int _responseTimeout;
        private readonly Action _callbackOnFailure;
        private System.Threading.Timer _requestTimer;
        private bool _gotRequest = false;
        private Guid _currentRequestId;
        private DateTime _requestStartTime;
        private readonly ILogger _logger;

        /// <summary>Creates new RequestWatchdog, callback will be never called on Informatica thread</summary>
        /// <param name="message">Message to send</param>
        /// <param name="responseCallback">
        ///     Callback that will be called if response is received with the right type on a new thread
        /// </param>
        /// <param name="request">Request delegate</param>
        /// <param name="additionalCallbackArgs">Argument that will be passed to request delegate</param>
        /// <param name="responseTimeout">Timeout in ms after which retry will occur</param>
        /// <param name="callbackOnFailure">Callback that will be called if RequestWatchdog gives up</param>
        public RequestWatchdog(
            IMessage message,
            Action<object, byte[]> responseCallback,
            Action<object, IMessage, LBMResponseCallback> request,
            object additionalCallbackArgs,
            int responseTimeout,
            Action callbackOnFailure)
        {
            _message = message;
            _responseCallback = responseCallback;
            _request = request;
            _additionalCallbackArgs = additionalCallbackArgs;
            _responseTimeout = responseTimeout;
            _callbackOnFailure = callbackOnFailure;

            if (_responseTimeout < 1500)
            {
                _responseTimeout = 1500;
            }

            MaxRetryAttempts = 5;
            _logger = new NLogLoggerFactory().GetCurrentClassLogger();
        }

        /// <summary>Starts sending request</summary>
        public void Start()
        {
            if (_requestTimer != null) // already started
                return;
            _requestStartTime = InternalTime.LocalNow();
            _message.RequestId = Guid.NewGuid();

            TrySendRequest();
            _requestTimer =
                new System.Threading.Timer(RequestTimeoutCallback, null, _responseTimeout, _responseTimeout);
        }

        private int _retryCount;

        /// <summary>
        ///     Tries to call request. If MaxRetryAttempts is reached, stops RequestWatchdog and calls CallbackOnFailure
        /// </summary>
        /// <param name="request"></param>
        private void RequestTimeoutCallback(object request)
        {
            if (!_gotRequest)
            {
                if (_retryCount < MaxRetryAttempts)
                {
                    _retryCount++;
                    TrySendRequest();
                }
                else
                {
                    DateTime failedTime = InternalTime.LocalNow();
                    Stop();
                    _logger.Warn(
                        "Request {0} failed after {1:F2}s",
                        _message.GetType().Name,
                        failedTime.Subtract(_requestStartTime).TotalSeconds);

                    _callbackOnFailure?.Invoke();
                }
            }
        }

        private void TrySendRequest()
        {
            _request(new Tuple<Guid, object>(_currentRequestId, _additionalCallbackArgs), _message, OnLBMResponse);
        }

        private int OnLBMResponse(object cbArg, LBMRequest request, LBMMessage response)
        {
            InformaticaLogger.LogRequestResponseHeader(_logger, nameof(OnLBMResponse), request, response);
            DateTime receivedTime = InternalTime.LocalNow();

            using (response)
            using (request)
            {
                return HandleResponse(response, receivedTime, cbArg as Tuple<Guid, object>);
            }
        }

        private int HandleResponse(LBMMessage response, DateTime receivedTime, Tuple<Guid, object> args)
        {
            if (_gotRequest)
            {
                return 0;
            }

            switch (response.type())
            {
                case LBM.MSG_RESPONSE:
                {
                    HandleInformaticaResponse(response, receivedTime, args);
                }
                break;

                default:
                break;
            }

            return 0;
        }

        private void HandleInformaticaResponse(LBMMessage response, DateTime receivedTime, Tuple<Guid, object> args)
        {
            _gotRequest = true;
            Stop();

            _logger.Trace(
                "Request {0} received in {1:F2}s",
                _message.GetType().Name,
                receivedTime.Subtract(_requestStartTime).TotalSeconds);

            InformaticaResponse.HandleInformaticaResponse(
                response.data(),
                (responseData) => _responseCallback?.Invoke(args?.Item2, responseData));
        }

        /// <summary>Stops sending request</summary>
        public void Stop()
        {
            _requestTimer.Dispose();
        }

        public int MaxRetryAttempts { get; set; }
    }
}