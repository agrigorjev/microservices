using AutoMapper;
using JetBrains.Annotations;
using Mandara.Business.Bus;
using Mandara.Business.Bus.Commands;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Contracts;
using Mandara.Entities;
using Mandara.Extensions.Option;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Mandara.Business.Services
{
    public class SecurityDefinitionsRemoteStorage : ISecurityDefinitionsStorage
    {
        private readonly CommandManager _commandManager;
        private readonly InformaticaHelper _informaticaHelper;
        private readonly ManualResetEventSlim _startReceiveEvent;
        private bool failed = false;
        private readonly ManualResetEventSlim _dataReceivedEvent;
        private readonly ILogger _log;
        private ConcurrentDictionary<int, SecurityDefinition> SecurityDefinitions;// { get; private set; }
        public int MaxId { get; private set; }

        public SecurityDefinitionsRemoteStorage(
            [NotNull] CommandManager commandManager,
            [NotNull] InformaticaHelper informaticaHelper,
            [NotNull] ILogger log)

        {
            _commandManager = commandManager;
            _informaticaHelper = informaticaHelper;
            _startReceiveEvent = new ManualResetEventSlim(false);
            _dataReceivedEvent = new ManualResetEventSlim(false);
            _log = log;
            SecurityDefinitions = new ConcurrentDictionary<int, SecurityDefinition>();
            MaxId = 0;
        }

        public void Update()
        {
            SecurityDefinitionsSnapshotMessageDto message = new SecurityDefinitionsSnapshotMessageDto();
            GetSnapshot(InformaticaHelper.SecurityDefinitionsSnapshotTopicName, message, SnapshotCallback);
            TimeSpan timeout = TimeSpan.FromSeconds(30);
            if (!_startReceiveEvent.Wait(timeout) || failed)
            {
                _log.Error("Cannot get response from Security Definitions service after {0} seconds", timeout.Seconds);
                throw new InvalidOperationException("Cannot connect to Security Definitions service");
            }
            if (!_dataReceivedEvent.Wait(timeout))
            {
                _log.Error("Cannot get data from Security Definitions service after {0} seconds", timeout.Seconds);
                throw new InvalidOperationException("Cannot receive Security Definitions from remote service");
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Clean()
        {
            throw new NotImplementedException();
        }

        public TryGetResult<SecurityDefinition> TryGetSecurityDefinition(int secDefId)
        {
            SecurityDefinition secDef;

            SecurityDefinitions.TryGetValue(secDefId, out secDef);
            return new TryGetRef<SecurityDefinition>() { Value = secDef };
        }

        public TryGetResult<SecurityDefinition> TryGetSecurityDefinitionBySymbol(string symbol)
        {
            throw new NotImplementedException();
        }

        public TryGetResult<SecurityDefinition> TryGetSecurityDefinitionByFields(
            Predicate<SecurityDefinition> secDefFilter)
        {
            throw new NotImplementedException();
        }

        public bool TryAdd(SecurityDefinition secDef)
        {
            return SecurityDefinitions.TryAdd(secDef.SecurityDefinitionId, secDef);
        }

        public IEnumerable<SecurityDefinition> GetSecurityDefinitions()
        {
            return SecurityDefinitions.Values;
        }

        public IEnumerable<SecurityDefinition> Where(Func<SecurityDefinition, bool> filter)
        {
            return SecurityDefinitions.Values.Where(filter).Select(security => security.ShallowCopy());
        }

        public void MarkSecurityDefinitionForReread(int secDefId)
        {
            throw new NotImplementedException();
        }

        public void CleanupNegativeIdSecurityDefinitions()
        {
            throw new NotImplementedException();
        }

        private void SnapshotReceivedCallback()
        {
            // we got initial response from SD service
            _startReceiveEvent.Set();
        }

        private void SnapshotCallback(List<SecurityDefinitionsSnapshotMessageDto> messages)
        {
            int startMaxId = MaxId;
            List<SecurityDefinition> securityDefinitions =
                messages
                    .Select(Mapper.Map<SecurityDefinitionsSnapshotMessage>)
                    .SelectMany(it => it.SecurityDefinitions)
                    .ToList();

            securityDefinitions.ForEach(sd => SecurityDefinitions.TryAdd(sd.SecurityDefinitionId, sd));
            MaxId = securityDefinitions.Last().SecurityDefinitionId;
            LogUpdateResult(startMaxId, securityDefinitions.Count);
            _dataReceivedEvent.Set();
        }

        private void LogUpdateResult(int startMaxId, int securityDefinitionsCount = 0)
        {
            if (MaxId != startMaxId)
            {
                _log.Info(
                    "Received Security Definitions with IDs from {0} to {1} ({2} records)",
                    startMaxId + 1,
                    MaxId,
                    securityDefinitionsCount);
            }
            else
            {
                _log.Info("No new Security Definitions were received.");
            }
        }

        private void SnapshotFailureCallback()
        {
            failed = true;
            _startReceiveEvent.Set();
        }
        private void GetSnapshot<TMessage>(string topicName, TMessage message, Action<List<TMessage>> callback)
            where TMessage : SnapshotMessageBase
        {
            message.UseGzip = true;
            _commandManager.AddCommand(
                new RequestSnapshotPackageCommand<TMessage>(
                    message,
                    topicName,
                    callback,
                    SnapshotReceivedCallback,
                    SnapshotFailureCallback,
                    _informaticaHelper));
        }
    }
}
