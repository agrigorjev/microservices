using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mandara.Business.Bus;
using Mandara.Business.Bus.Commands;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Client.AsyncServices.Heartbeat;
using Mandara.Date.Time;
using Mandara.Entities.ErrorReporting;
using Ninject.Extensions.Logging;

namespace Mandara.Business.AsyncServices
{
    public class HeartbeatClientService : ClientAsyncService
    {
        public const int DefaultErrorPeriodSeconds = 20;
        private readonly TimeSpan _timeUntilHeartbeatIsLost;
        public const int DefaultHeartbeatCheckSeconds = 10;
        private readonly TimeSpan _heartBeatCheckSeconds;
        private DateTime _lastNoHeartbeatAtUtc = InternalTime.UtcNow();
        private bool _isHeartbeatOk = true;
        // A sequence number equal to -1 indicates that sequence numbers should be reset
        private const int InvalidSequence = -1;
        private const int SequenceStart = 0;

        public HeartbeatClientService(
            TimeSpan heartbeatCheck, 
            TimeSpan heartbeatTimeout, 
            CommandManager commandManager, 
            BusClient busClient, 
            ILogger log) : base(
            commandManager,
            busClient,
            log)
        {
            _heartBeatCheckSeconds = heartbeatCheck;
            _timeUntilHeartbeatIsLost = heartbeatTimeout;
            SleepTime = _heartBeatCheckSeconds;

            BusClient.SequenceReset -= OnSequenceReset;
            BusClient.SequenceReset += OnSequenceReset;
        }

        private void OnSequenceReset(object sender, EventArgs eventArgs)
        {
            _log.Warn("Resetting client sequence numbers");
            BusClient.PendingTradesUpdates.ResetSequence();
            BusClient.PendingPositionsUpdates.ResetSequence();
            BusClient.PendingFxExposureDetailUpdates.ResetSequence();
        }

        protected override void OnStopped()
        {
            base.OnStopped();

            BusClient.SequenceReset -= OnSequenceReset;
        }

        protected override void DoWork()
        {
            _log.Trace("Check Server Heartbeat");
            ServerHeartbeat message = BusClient.LastHeartbeat ?? ServerHeartbeat.Default;
            bool prevHeartbeatOk = _isHeartbeatOk;

            _isHeartbeatOk = CheckServerHeartbeat.IsHeartbeatOk(message.LastHeartbeatUtc, _heartBeatCheckSeconds);

            if (!_isHeartbeatOk)
            {
                HandleFailedHeartbeatCheck(prevHeartbeatOk);
            }

            CheckSequence(
                BusClient.PendingTradesUpdates,
                message.LiveTradesLastSequence,
                InformaticaHelper.TradesUpdateTopicName);

            CheckSequence(
                BusClient.PendingPositionsUpdates,
                message.LivePositionsLastSequence,
                InformaticaHelper.PositionsUpdateTopicName);

            CheckSequence(
                BusClient.PendingFxExposureDetailUpdates,
                message.LiveFxExposureDetailLastSequence,
                InformaticaHelper.FxExposureDetailUpdateTopicName);
        }

        private void HandleFailedHeartbeatCheck(bool prevHeartbeatOk)
        {
            if (prevHeartbeatOk)
            {
                _lastNoHeartbeatAtUtc = InternalTime.UtcNow();
                _log.Warn("Check Server Heartbeat - no heartbeat received between check");

                return;
            }

            if (!CheckServerHeartbeat.IsHeartbeatLost(_lastNoHeartbeatAtUtc, _timeUntilHeartbeatIsLost))
            {
                _log.Warn("Check Server Heartbeat - still no heartbeat");
                return;
            }

            _lastNoHeartbeatAtUtc = InternalTime.UtcNow();

            var err = new Error(
                "Server",
                ErrorType.Exception,
                "Server not responding",
                level: ErrorLevel.Critical);
            
            _log.Error(err.Message);            
            ErrorReportingHelper.GlobalQueue.Enqueue(err);

            StartReconnection();
        }

        private void StartReconnection()
        {
            BusClient.OnConnectionLost();

            // reset sequence numbers because on another server they may be completely different e.g., it was 3 and 5 on
            // server One and it is 45 and 47 on server Two just because server Two was started earlier
            OnSequenceReset(this, EventArgs.Empty);
        }

        private void CheckSequence<T>(SequenceQueue<T> sequenceQueue, int lastKnownSequenceNumber, string topicName)
            where T : class, IMessage
        {
            if (InvalidSequence == lastKnownSequenceNumber)
            {
                sequenceQueue.ResetSequence();
                return;
            }

            if (lastKnownSequenceNumber > SequenceStart)
            {
                ReportSkippedSequences(sequenceQueue, lastKnownSequenceNumber, topicName);
            }
        }

        private void ReportSkippedSequences<T>(
            SequenceQueue<T> sequence,
            int lastKnownSequenceNumber,
            string topicName) where T : class, IMessage
        {
            List<int> skippedSequences =
                CheckServerHeartbeat.SkippedSequenceNumbers(sequence, lastKnownSequenceNumber);

            if (skippedSequences.Count <= 0)
            {
                return;
            }

            Debug.WriteLine(
                "Requesting live feed replay, topic [{0}], sequences [{1}]",
                topicName,
                string.Join(", ", skippedSequences));

            RunCommand(new LiveFeedDeliveryCommand(topicName, skippedSequences));
        }
    }
}