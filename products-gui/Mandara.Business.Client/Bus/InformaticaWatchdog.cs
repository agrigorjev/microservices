using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using com.latencybusters.lbm;
using Mandara.Business.AsyncServices.Base;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Date.Time;

namespace Mandara.Business.Bus
{
    public partial class InformaticaHelper
    {
        /// <summary>
        /// _requestWatchdogCache is a cache of request responses. 
        /// When client receives no response it retries request. If server already received 
        /// that request once it should not handle it twice. Instead it should send request 
        /// results to the client. These results are stored in _requestWatchdogCache.
        /// </summary>
        private static readonly ConcurrentDictionary<Guid, RequestWatchdogCacheRecord> _requestWatchdogCache =
            new ConcurrentDictionary<Guid, RequestWatchdogCacheRecord>();

        private static readonly RequestWatchdogCacheRecord RequestAwaitingProcessing = null;

        /// <summary>
        /// Detects if LBMMessage should be handled by watchdog. Then handles message and returns 
        /// true if it should, and returns false if it should not.
        /// </summary>
        /// <param name="message">LBMMessage received</param>
        /// <returns></returns>
        public static bool HandleByWatchdog(LBMMessage message)
        {
            if (message.type() != LBM.MSG_REQUEST)
            {
                return false;
            }

            return HandleByWatchdog(
                JsonHelper.Deserialize<MessageBase>(message.data()),
                ResendExistingResponse(message));
        }

        private static bool HandleByWatchdog(
            MessageBase message,
            Action<RequestWatchdogCacheRecord> repeatExistingResponse)
        {
            if (IsMessageValidForWatchdog(message))
            {
                return false;
            }

            ;
            (bool alreadyHandled, RequestWatchdogCacheRecord cacheRecord) = GetExistingCacheRecord(message);

            if (IsRequestedAndAwaitingProcessing(cacheRecord))
            {
                LogRequestCacheStatus(message, alreadyHandled);
                return alreadyHandled;
            }

            _log.Info("Responding from watchdog cache [{0}]", message.RequestId);
            cacheRecord.UpdateLastResponseTime();
            repeatExistingResponse(cacheRecord);
            return true;
        }

        private static bool IsMessageValidForWatchdog(MessageBase message)
        {
            return message == null || message.RequestId == Guid.Empty;
        }

        private static (bool, RequestWatchdogCacheRecord) GetExistingCacheRecord(MessageBase message)
        {
            return _requestWatchdogCache.TryGetValue(message.RequestId, out RequestWatchdogCacheRecord cachedRequest)
                ? (true, cachedRequest)
                : AddOrUpdateRequestInCache(message);
        }

        private static (bool, RequestWatchdogCacheRecord) AddOrUpdateRequestInCache(MessageBase message)
        {
            _log.Info("Updating existing or creating new cache record for request [{0}]", message.RequestId);

            bool isInWatchdogCache = false;
            RequestWatchdogCacheRecord cachedRequest = _requestWatchdogCache.AddOrUpdate(
                message.RequestId,
                RequestAwaitingProcessing,
                (key, existingRequest) =>
                {
                    isInWatchdogCache = true;
                    return existingRequest;
                });

            return (isInWatchdogCache, cachedRequest);
        }

        private static bool IsRequestedAndAwaitingProcessing(RequestWatchdogCacheRecord cacheRecord)
        {
            return RequestAwaitingProcessing == cacheRecord;
        }

        private static void LogRequestCacheStatus(MessageBase message, bool alreadyHandled)
        {
            if (alreadyHandled)
            {
                _log.Info("Cache record already created for request [{0}].  Awaiting processing.", message.RequestId);
            }
            else
            {
                _log.Info($"New request cache record created for request [{message.RequestId}].");
            }
        }

        private static Action<RequestWatchdogCacheRecord> ResendExistingResponse(LBMMessage message)
        {
            return (cachedRequest) =>
            {
                //RespondToMessage(message, cachedRequest.Data);
                RespondToMessage(InformaticaHelper.BuildResponder(message), cachedRequest.Data);
                message.Dispose();
            };
        }

        /// <summary>
        /// RequestWatchdogCacheRecord is a class that stores single response data in 
        /// watchdog request cache
        /// </summary>
        private class RequestWatchdogCacheRecord
        {
            public RequestWatchdogCacheRecord(byte[] data)
            {
                Data = data;
                UpdateLastResponseTime();
            }

            /// <summary>
            /// Cached response data
            /// </summary>
            public byte[] Data { get; private set; }

            /// <summary>
            /// Last time the Data was requested
            /// </summary>
            public DateTime LastResponseTime { get; private set; }

            /// <summary>
            /// Updates LastResponseTime to current time
            /// </summary>
            public void UpdateLastResponseTime()
            {
                LastResponseTime = SystemTime.Now();
            }
        }

        private static TimeSpan _requestWatchdogCacheRecordTTL;

        private AsyncService _requestWatchdogCacheCleaningService;
        // Service that cleans watchdog cache records older than _requestWatchdogCacheRecordTTL

        /// <summary>
        /// Cleans watchdog cache records older than _requestWatchdogCacheRecordTTL
        /// </summary>
        private static void CleanRequestWatchdogCache()
        {
            List<Guid> oldRecords = new List<Guid>();
            foreach (Guid id in _requestWatchdogCache.Keys)
            {
                if (!_requestWatchdogCache.TryGetValue(id, out RequestWatchdogCacheRecord value))
                {
                    continue;
                }

                if (value == null)
                {
                    continue;
                }

                if ((SystemTime.Now() - value.LastResponseTime) > _requestWatchdogCacheRecordTTL)
                {
                    oldRecords.Add(id);
                }
            }

            foreach (Guid id in oldRecords)
            {
                if (_requestWatchdogCache.TryRemove(id, out RequestWatchdogCacheRecord value))
                {
                    _log.Info(string.Format("removed watchdog cache entry [{0}]", id));
                }
            }
        }

        private void StartRequestWatchdogCacheCleaningService()
        {
            _requestWatchdogCacheRecordTTL = TimeSpan.FromMinutes(5);
            _requestWatchdogCacheCleaningService = AsyncServiceFactory.FromAction(CleanRequestWatchdogCache, _log);
            _requestWatchdogCacheCleaningService.Start();
        }

        private void CloseCleanUpService()
        {
            _requestWatchdogCacheCleaningService.Stop();
        }
    }
}
