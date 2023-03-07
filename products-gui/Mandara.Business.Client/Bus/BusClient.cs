using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using AutoMapper;
using com.latencybusters.lbm;
using Mandara.Business.AsyncServices;
using Mandara.Business.AsyncServices.Base;
using Mandara.Business.Authorization;
using Mandara.Business.Bus.BusClientParts;
using Mandara.Business.Bus.Commands;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Handlers;
using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.Messages.Audit;
using Mandara.Business.Bus.Messages.Authorization;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Bus.Messages.CategoryInfo;
using Mandara.Business.Bus.Messages.Currencies;
using Mandara.Business.Bus.Messages.DailyReconciliation;
using Mandara.Business.Bus.Messages.DataValidation;
using Mandara.Business.Bus.Messages.Derisking;
using Mandara.Business.Bus.Messages.EditTrades;
using Mandara.Business.Bus.Messages.Export;
using Mandara.Business.Bus.Messages.Features;
using Mandara.Business.Bus.Messages.Fx;
using Mandara.Business.Bus.Messages.Historical;
using Mandara.Business.Bus.Messages.Pnl;
using Mandara.Business.Bus.Messages.Portfolio;
using Mandara.Business.Bus.Messages.Positions;
using Mandara.Business.Bus.Messages.PricingReport;
using Mandara.Business.Bus.Messages.Settings;
using Mandara.Business.Bus.Messages.Spreader;
using Mandara.Business.Bus.Messages.TradeAdd;
using Mandara.Business.Bus.Messages.TradeAssign;
using Mandara.Business.Bus.Messages.TradeImpact;
using Mandara.Business.Bus.Messages.Trades;
using Mandara.Business.Bus.Messages.TransferErrors;
using Mandara.Business.Cache.EventArgs;
using Mandara.Business.Client.Bus.BusClientParts;
using Mandara.Business.Client.Managers;
using Mandara.Business.Config;
using Mandara.Business.Managers;
using Mandara.Business.TradeAdd;
using Mandara.Entities;
using Mandara.Entities.Calculation;
using Mandara.Entities.Enums;
using Mandara.Entities.ErrorDetails;
using Mandara.Entities.ErrorReporting;
using Mandara.Entities.Import;
using Mandara.Entities.MatchingDummies;
using Mandara.Entities.Trades;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;

namespace Mandara.Business.Bus
{
    public class BusClient : IDisposable
    {
        private const int DefaultHistoricalRiskDays = 60;
        public static BusClient Instance => IoC.Get<BusClient>();

        private string LBMConfigurationFilePath => ConfigurationManager.AppSettings["LBMConfigurationFilePath"];

        private TimeSpan GetHeartbeatErrorPeriod() => TimeSpan.FromSeconds("HeartbeatErrorPeriod_Seconds".GetSetting()
            .Match(
                setting => int.TryParse(setting, out int heartbeatErrorPeriod)
                    ? heartbeatErrorPeriod
                    : HeartbeatClientService.DefaultErrorPeriodSeconds,
                err =>
                {
                    _log.Warn(err.Message);
                    return HeartbeatClientService.DefaultErrorPeriodSeconds;
                }));

        private TimeSpan GetHeartbeatCheckInterval() => TimeSpan.FromSeconds("HeartbeatCheckInterval_Seconds"
            .GetSetting()
            .Match(
                setting => int.TryParse(setting, out int heartbeatErrorPeriod)
                    ? heartbeatErrorPeriod
                    : HeartbeatClientService.DefaultHeartbeatCheckSeconds,
                err =>
                {
                    _log.Warn(err.Message);
                    return HeartbeatClientService.DefaultHeartbeatCheckSeconds;
                }));

        public LBMContext LbmContext { get; private set; }

        private readonly ReaderWriterLockSlim _lockPnl = new ReaderWriterLockSlim();

        public event EventHandler<TradesChangedEventArgs> TradesChanged;
        public event EventHandler<FxTradesChangedEventArgs> FxTradesChanged;
        public event EventHandler<FxExposureDetailChangedEventArgs> FxExposureDetailsChanged;
        public event EventHandler<PortfoliosChangedEventArgs> PortfoliosChanged;
        public event EventHandler<MatchingDummiesChangedEventArgs> MatchingDummiesChanged;
        public event EventHandler<RolloffNotificationEventArgs> RolloffNotification;
        public event EventHandler<TasCheckErrorsEventArgs> TasCheckErrorsNotification;
        public event EventHandler<TradeSupportEventArgs> TradeSupportAcknowledged;
        public event EventHandler<VarLatestUpdateHandler.VarDataEventArgs> VarLatestUpdated;
        public event EventHandler<PnlDiffUpdateHandler.PnlDiffEventArgs> PnlDiffUpdated;
        public event EventHandler<TradeSupportEventArgs> TradeSupportAdded;
        public event EventHandler<ServerReconnectionEventArgs> ServerReconnectionNeeded;
        public event EventHandler ServerConnectionLost;
        public event EventHandler SequenceReset;
        public event EventHandler PositionChanged;

        private AsyncServiceManager _serviceManager;
        public InformaticaHelper InformaticaHelper { get; private set; }

        public ServerHeartbeat LastHeartbeat { get; set; }

        public SequenceQueue<TradesUpdateMessage> PendingTradesUpdates { get; private set; }
        public SequenceQueue<FxTradesUpdateMessage> PendingFxTradesUpdates { get; private set; }
        public SequenceQueue<PositionsUpdateMessage> PendingPositionsUpdates { get; private set; }
        public SequenceQueue<FxExposureDetailUpdateMessage> PendingFxExposureDetailUpdates { get; private set; }

        private readonly ReaderWriterLockSlim _lockLiveData = new ReaderWriterLockSlim();
        private List<LiveData> _liveData;
        private UserData _userData;
        private readonly ReaderWriterLockSlim _lockUserData = new ReaderWriterLockSlim();
        private readonly UserPrivileges _privileges;

        public List<LiveData> LiveData
        {
            get
            {
                try
                {
                    _lockLiveData.EnterReadLock();

                    if (_liveData != null)
                    {
                        return _liveData.ToList();
                    }

                    return null;
                }
                finally
                {
                    _lockLiveData.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    _lockLiveData.EnterWriteLock();
                    _liveData = value;
                }
                finally
                {
                    _lockLiveData.ExitWriteLock();
                }
            }
        }

        public decimal? OvernightAbnPnl { get; set; }

        public ConcurrentDictionary<int, Portfolio> Portfolios { get; private set; }
        public List<RolloffDetail> RolloffDetails { get; set; }

        //This method to handle root book selection during binding of treelist.
        //Please use that one for binding
        public List<Portfolio> SortedPortfolios => Portfolios.Values.OrderBy(p => p.Name).ToList();

        public ConcurrentDictionary<string, CalculationDetail> Positions { get; }

        public List<CalculationDetail> GetPositions()
        {
            return Positions.Values.ToList();
        }

        public Dictionary<int, Dictionary<string, PnlData>> PnlDictionary
        {
            get => _pnlSource.PnlDictionary;
            set => _pnlSource.PnlDictionary = value;
        }

        public ConcurrentDictionary<Guid, TradeSupportAlert> TradeSupportAlerts { get; private set; }
        private readonly ConcurrentDictionary<string, Predicate<TradeSupportAlert>> _tradeSupportFilters;
        private Dictionary<Type, Func<TradeSupportAlert, bool>> _addTradeSupportAlertPredicates;

        private List<TasCheckDetail> _currentTasErrors;
        private static readonly ReaderWriterLockSlim LockTasErrors = new ReaderWriterLockSlim();

        public DateTime? OnPnlSnapshotDatetime { get; set; }

        public bool PositionsReceived { get; private set; }

        private readonly PnL _pnlSource;

        private static int ConfiguredLogPeriod = Int32.MinValue;

        /// <summary>Get period (in dates) for trade support log.</summary>
        public int TradeSupportLogPeriod
        {
            get
            {
                if (ConfiguredLogPeriod == Int32.MinValue)
                {
                    ConfiguredLogPeriod = int.TryParse(
                        ConfigurationManager.AppSettings["TradeSupportLogDays"] ?? "0",
                        out int configuredLogDays)
                        ? configuredLogDays
                        : 0;
                }

                return ConfiguredLogPeriod;
            }
        }

        public int MaxHistoricalRiskDays
        {
            get
            {
                if (_maxHistoricalRiskDays == 0)
                {
                    _maxHistoricalRiskDays = DefaultHistoricalRiskDays;
                }

                return _maxHistoricalRiskDays;
            }
            set => _maxHistoricalRiskDays = value;
        }

        public BusClient()
        {
            _log = new NLogLogger(typeof(BusClient));

            _privileges = new UserPrivileges(this);
            PendingTradesUpdates = new SequenceQueue<TradesUpdateMessage>();
            PendingFxTradesUpdates = new SequenceQueue<FxTradesUpdateMessage>();
            PendingPositionsUpdates = new SequenceQueue<PositionsUpdateMessage>();
            PendingFxExposureDetailUpdates = new SequenceQueue<FxExposureDetailUpdateMessage>();

            TradeSupportAlerts = new ConcurrentDictionary<Guid, TradeSupportAlert>();
            _tradeSupportFilters = new ConcurrentDictionary<string, Predicate<TradeSupportAlert>>();

            Positions = new ConcurrentDictionary<string, CalculationDetail>();
            LiveData = new List<LiveData>();
            Portfolios = new ConcurrentDictionary<int, Portfolio>();

            //Initialize class used to collect update panel data for fututre to get throttled value if needed.
            _pnlSource = new PnL();
            TurnOnErrorsReceiver = true;
            TurnOnMatchingDummies = true;

            try
            {
                LockTasErrors.EnterWriteLock();
                _currentTasErrors = new List<TasCheckDetail>();
            }
            finally
            {
                LockTasErrors.ExitWriteLock();
            }

            SetUpTradeSupportAlertPredicates();
        }

        private void SetUpTradeSupportAlertPredicates()
        {
            _addTradeSupportAlertPredicates = new Dictionary<Type, Func<TradeSupportAlert, bool>>
            {
                { typeof(List<TasCheckDetail>), HasPermissionForTasCheckPortfolios }
            };
        }

        /// <summary>Returns strored throttled value for LivePnl value</summary>
        /// <param name="portfolioId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public Money GetLivePnlThrottled(int portfolioId, string currency)
        {
            return _pnlSource.GetLivePnlThrottled(portfolioId, currency);
        }

        public void Start()
        {
            if (null == LbmContext)
            {
                InitialiseMessageBus();

                InformaticaHelper.CreateSources();
                InformaticaHelper.CreateReceivers();
                InitialiseBusClientServices();
            }

            // RefreshLivePositions();
            OnSequenceReset();
            RequestPortfolio();

            if (TurnOnMatchingDummies)
            {
                GetMatchingDummiesSnapshot(new MatchingDummiesMessageArgs(), Match_Callback);
            }

            _privileges.StartPrivilegeWorker();

            GetSettingsFromServer((info) => { });
            GetFeatures(Features_Callback, (info) => { });
            GetCurrencies(Currencies_Callback, (info) => { });
        }

        public void InitialiseMessageBus()
        {
            if (LBMConfigurationFilePath != null)
            {
                if (File.Exists(LBMConfigurationFilePath))
                {
                    LBM.setConfiguration(LBMConfigurationFilePath);
                }
            }

            InitialiseMessageBus(new LBMContext());
        }

        public void InitialiseMessageBus(LBMContext msgContext)
        {
            if (null != LbmContext)
            {
                _log.Error("BusClient: Attempt to set the LBMContext when it is already set.");
                return;
            }

            LbmContext = msgContext;

            CommandManager = new CommandManager();
            CommandManager.Start();

            InformaticaHelper = new ClientInformaticaHelper(LbmContext, new HandlerManager(), TurnOnErrorsReceiver);
        }

        public void InitialiseBusClientServices()
        {
            _serviceManager = new AsyncServiceManager();
            _serviceManager.AddRange(
                new AsyncService[]
                {
                    new HeartbeatClientService(
                        GetHeartbeatCheckInterval(), 
                        GetHeartbeatErrorPeriod(), 
                        CommandManager, 
                        this, 
                        _log),
                    new TradesUpdateClientService(CommandManager, this, _log),
                    new FxTradesUpdateClientService(CommandManager, this, _log),
                    new PositionsUpdateClientService(CommandManager, this, _log),
                    new FxExposureDetailUpdateClientService(CommandManager, this, _log),
                    new ClientHeartbeatClientService(ClientGuidHolder.ClientGuid, CommandManager, this, _log),
                });
            _serviceManager.Start();
        }

        public UserPrivileges GetPrivileges()
        {
            return _privileges;
        }

        public void RequestPortfolio()
        {
            GetPortfolios(Portfolios_Callback);
        }

        public bool TurnOnErrorsReceiver { get; set; }

        public bool TurnOnMatchingDummies { get; set; }

        private void GetFeatures(Action<FeaturesResponseMessage> responseCallback, Action<FailureCallbackInfo> timeoutCallback)
        {
            CommandManager.AddCommand(
                new SendRequestCommand<FeaturesRequestMessage, FeaturesResponseMessage>(
                    InformaticaHelper.FeaturesTopicName,
                    new FeaturesRequestMessage(),
                    responseCallback,
                    5000,
                    timeoutCallback));
        }

        private void Features_Callback(FeaturesResponseMessage message)
        {
            if (message == null)
            {
                return;
            }

            FeatureManager.Init(message.Features);
        }

        private void GetCurrencies(
            Action<CurrenciesResponseMessage> responseCallback,
            Action<FailureCallbackInfo> timeoutCallback)
        {
            CommandManager.AddCommand(
                new SendRequestCommand<CurrenciesRequestMessage, CurrenciesResponseMessage>(
                    InformaticaHelper.CurrenciesTopicName,
                    new CurrenciesRequestMessage(),
                    responseCallback,
                    5000,
                    timeoutCallback));
        }

        private void Currencies_Callback(CurrenciesResponseMessage message)
        {
            if (message == null)
            {
                return;
            }

            CurrencyManager.Init(message.Currencies);
        }

        private void GetSettingsFromServer(Action<FailureCallbackInfo> timeoutCallback)
        {
            CommandManager.AddCommand(
                new SendRequestCommand<SettingsRequestMessage, SettingsResponseMessage>(
                    InformaticaHelper.SettingsTopicName,
                    new SettingsRequestMessage(),
                    SettingsCallback,
                    timeoutCallback));
        }

        private void SettingsCallback(SettingsResponseMessage message)
        {
            this.MaxHistoricalRiskDays = message.MaxHistoricalRiskDays;
        }

        public void GetTasCheckErrorsSnapshot(Action<FailureCallbackInfo> timeoutCallback)
        {
            CommandManager.AddCommand(
                new SendRequestCommand<TasCheckSnapshotRequestMessage, TasCheckDetailResponseMessage>(
                    InformaticaHelper.TasCheckSnapshotTopicName,
                    new TasCheckSnapshotRequestMessage(),
                    TasCheckErrorsSnapshotCallback,
                    timeoutCallback));
        }

        private void TasCheckErrorsSnapshotCallback(TasCheckDetailResponseMessage message)
        {
            if (message == null)
            {
                return;
            }

            OnTasCheckUpdate(message.TasCheckDetails);
        }

        /// <summary>Returns last recieved TASErrors collection</summary>
        public List<TasCheckDetail> CurrentTASErrors
        {
            get
            {
                try
                {
                    LockTasErrors.EnterReadLock();
                    return _currentTasErrors.ToList();
                }
                finally
                {
                    LockTasErrors.ExitReadLock();
                }
            }
        }

        private void Portfolios_Callback(PortfolioResponseMessage message)
        {
            _log.Info("Portfolios received");

            if (message?.Portfolios != null)
            {
                UpdatePortfolios(message.Portfolios);
            }
        }

        public CommandManager CommandManager { get; private set; }

        public void OvernightPnlSnapshotChanged(DateTime snapshotDatetime)
        {
            ChangeSnapshotTime(snapshotDatetime, m => { }, (info) => { });
        }

        public void OnTradesChanged(TradeCapture tradeCapture)
        {
            if (TradesChanged != null)
            {
                TradesChangedEventArgs args = new TradesChangedEventArgs(tradeCapture);

                TradesChanged(this, args);
            }
        }

        public void OnFxTradesChanged(FxTrade fxTrade)
        {
            if (FxTradesChanged != null)
            {
                FxTradesChangedEventArgs args = new FxTradesChangedEventArgs(fxTrade);

                FxTradesChanged(this, args);
            }
        }

        public void OnFxExposureDetailChanged(FxExposureDetail detail)
        {
            if (FxExposureDetailsChanged != null)
            {
                FxExposureDetailChangedEventArgs args = new FxExposureDetailChangedEventArgs(detail);

                FxExposureDetailsChanged(this, args);
            }
        }

        public void Stop()
        {
            try
            {
                if (_serviceManager != null)
                {
                    _serviceManager.Stop();
                }

                if (CommandManager != null)
                {
                    CommandManager.Stop();
                }

                if (InformaticaHelper != null)
                {
                    InformaticaHelper.Close();
                }

                _privileges.Stop();

                if (LbmContext != null)
                {
                    LbmContext.close();
                    LbmContext = null;
                }

                ErrorReportingHelper.Close();
            }
            catch
            {
            }
        }

        public void GetTradesForPositions(
            DateTime? dailyDate,
            DateTime? riskDate,
            List<string> positionKeys,
            List<DateTime> days,
            Action<List<PositionsTradesMessage>> responseCallback)
        {
            PositionsTradesMessage message = new PositionsTradesMessage
            {
                DailyDate = dailyDate,
                RiskDate = riskDate,
                PositionsKeys = positionKeys,
                DailyTradesDays = days
            };

            GetSnapshot(InformaticaHelper.PositionsTradesTopicName, message, responseCallback);
        }

        public void GetTradesForExposureDetails(
            int portfolioId,
            int currencyId,
            List<DateTime> calculationDates,
            Action<List<FxExposureTradesMessage>> responseCallback)
        {
            FxExposureTradesMessage message = new FxExposureTradesMessage()
            {
                PortfolioId = portfolioId,
                CurrencyId = currencyId,
                CalculationDates = calculationDates
            };

            GetSnapshot(InformaticaHelper.FxExposureTradesTopicName, message, responseCallback);
        }

        public void GetFxExposureDetail(Action<List<FxExposureDetailsSnapshotMessage>> responseCallback)
        {
            FxExposureDetailsSnapshotMessage message = new FxExposureDetailsSnapshotMessage();
            GetSnapshot(InformaticaHelper.FxExposureDetailSnapshotTopicName, message, responseCallback);
        }

        public void GetFxTradesSnapshot(
            DateTime? startDate,
            DateTime? endDate,
            Action<List<FxTradesSnapshotMessage>> responseCallback)
        {
            FxTradesSnapshotMessage message = new FxTradesSnapshotMessage
            {
                StartDate = startDate,
                EndDate = endDate
            };

            GetSnapshot(
                InformaticaHelper.FxTradesSnapshotTopicName,
                message,
                responseCallback);
        }

        public void GetTradesSnapshot(
            DateTime? startDate,
            DateTime? endDate,
            Action<List<TradesSnapshotMessage>> responseCallback)
        {
            TradesSnapshotMessage message = TradesSnapshotMessage.ForDateRange(startDate, endDate);

            GetTradesSnapshot(message, responseCallback);
        }

        public void GetTradesSnapshot(int spreadGroupTradeId, Action<List<TradesSnapshotMessage>> responseCallback)
        {
            TradesSnapshotMessage message = TradesSnapshotMessage.ForSpreadGroup(spreadGroupTradeId);

            GetTradesSnapshot(message, responseCallback);
        }

        public void GetTradesSnapshot(List<int> tradeIds, Action<List<TradesSnapshotMessage>> responseCallback)
        {
            TradesSnapshotMessage message = TradesSnapshotMessage.ForTrades(tradeIds);

            GetTradesSnapshot(message, responseCallback);
        }

        public void GetTradesInGroupsSnapshot(List<int> groupIds, Action<List<TradesSnapshotMessage>> responseCallback)
        {
            TradesSnapshotMessage message = TradesSnapshotMessage.ForTradeGroups(groupIds);

            GetTradesSnapshot(message, responseCallback);
        }

        private void GetTradesSnapshot(TradesSnapshotMessage message, Action<List<TradesSnapshotMessage>> responseCallback)
        {
            _log.Debug("Trades snapshot: {0}", message);
            GetSnapshot<TradesSnapshotMessage, TradesSnapshotMessageDto>(
                InformaticaHelper.TradesSnapshotTopicName,
                message,
                responseCallback);
        }

        public void GetSnapshot<TMessage>(string topicName, TMessage message, Action<List<TMessage>> responseCallback)
            where TMessage : SnapshotMessageBase
        {
            message.UseGzip = true;

            CommandManager.AddCommand(new RequestSnapshotPackageCommand<TMessage>(message, topicName, responseCallback));
        }

        public void GetSnapshot<TMessage>(
            string topicName,
            TMessage message,
            Action<List<TMessage>> responseCallback,
            Action timeoutCallback) where TMessage : SnapshotMessageBase
        {
            message.UseGzip = true;

            CommandManager.AddCommand(
                new RequestSnapshotPackageCommand<TMessage>(
                    message,
                    topicName,
                    responseCallback,
                    RequestSnapshotPackageCommand<TMessage>.DefaultResponseReceivedHandler,
                    timeoutCallback,
                    null));
        }

        public void GetSnapshot<TMessage, TMessageDto>(
            string topicName,
            TMessage message,
            Action<List<TMessage>> responseCallback) where TMessage : SnapshotMessageBase
            where TMessageDto : SnapshotMessageBase
        {
            message.UseGzip = true;

            CommandManager.AddCommand(
                new RequestSnapshotDtoPackageCommand<TMessage, TMessageDto>(message, topicName, responseCallback));
        }

        public void GetSnapshot<TMessage>(string topicName, TMessage message, Action<TMessage> responseCallback)
            where TMessage : SnapshotMessageBase
        {
            message.UseGzip = true;

            CommandManager.AddCommand(new RequestSnapshotCommand<TMessage>(message, topicName, responseCallback));
        }

        public void GetSnapshot<TMessage, TMessageDto>(string topicName, TMessage message, Action<TMessage> responseCallback)
            where TMessage : SnapshotMessageBase where TMessageDto : SnapshotMessageBase
        {
            message.UseGzip = true;

            CommandManager.AddCommand(
                new RequestSnapshotDtoCommand<TMessage, TMessageDto>(message, topicName, responseCallback));
        }

        public void GetExpiringProductsSnapshot(
            ExpiringProductsSnapshotMessage message,
            Action<ExpiringProductsSnapshotMessage> responseCallback)
        {
            GetSnapshot(InformaticaHelper.ExpiringProductsSnapshotTopicName, message, responseCallback);
        }

        public void GetDataValidation(
            Action<DataValidationResponseMessage> responseCallback,
            Action<FailureCallbackInfo> failureCallback)
        {
            SendRequestCommand<MessageBase, DataValidationResponseMessage> requestCmd =
                new SendRequestCommand<MessageBase, DataValidationResponseMessage>(
                    InformaticaHelper.DataValidationSnapshotTopicName,
                    new MessageBase(),
                    responseCallback,
                    failureCallback);

            CommandManager.AddCommand(requestCmd);
        }

        public void GetTransferErrors(
            DateTime? lastDate,
            Action<TransferErrorsResponseMessage> responseCallback,
            Action<FailureCallbackInfo> failureCallback)
        {
            TransferErrorsRequestMessage requestMessage = new TransferErrorsRequestMessage() { LastDate = lastDate };

            var requestCmd = new SendRequestCommand<TransferErrorsRequestMessage, TransferErrorsResponseMessage>(
                InformaticaHelper.TransferErrorsTopicName,
                requestMessage,
                responseCallback,
                failureCallback);

            CommandManager.AddCommand(requestCmd);
        }

        public void GetTasTrades(bool zeroOnly, Action<List<TasPriceMessage>> responseCallback)
        {
            TasPriceMessage message = new TasPriceMessage { ZeroOnly = zeroOnly };
            GetSnapshot(InformaticaHelper.TasTradesTopicName, message, responseCallback);
        }

        public void SetTasPrices(Action<MessageStatusCode> responseCallback, List<PriceChanges> changedPrices)
        {
            CommandManager.AddCommand(new TasUpdateTradesCommand(changedPrices, responseCallback, GetUserData().UserName));
        }

        public void UpdateFromCSVSource(
            Action<MessageStatusCode> responseCallback,
            string fName,
            DateTime eDate,
            SourceDataType dataType)
        {
            CommandManager.AddCommand(new CsvImportClientCommand(fName, eDate, dataType, responseCallback));
        }

        public void GetHistoricalPositionsSnapshot(
            DateTime sourceDataDate,
            DateTime riskDate,
            SourceDataType dataType,
            string accountNumber,
            string exchangeValue,
            Action<List<HistoricalPositionsSnapshotMessage>> responseCallback)
        {
            HistoricalPositionsSnapshotMessage message = new HistoricalPositionsSnapshotMessage
            {
                SourceDataDate = sourceDataDate,
                RiskDate = riskDate,
                DataType = dataType,
                AccountNumber = accountNumber,
                ExchangeValue = exchangeValue
            };

            GetSnapshot<HistoricalPositionsSnapshotMessage, HistoricalPositionsSnapshotMessageDto>(
                InformaticaHelper.HistoricalPositionsSnapshotTopicName,
                message,
                responseCallback);
        }

        public void GetHistoricalParams(Action<List<HistoricalParamsMessage>> responseCallback)
        {
            GetSnapshot(InformaticaHelper.HistoricalParamsTopicName, new HistoricalParamsMessage(), responseCallback);
        }

        public void GetPortfolios(Action<PortfolioResponseMessage> responseCallback)
        {
            SendRequestDtoCommand<MessageBase, MessageBase, PortfolioResponseMessage,
                PortfolioResponseMessageDto> requestCmd =
                new
                    SendRequestDtoCommand<MessageBase, MessageBase, PortfolioResponseMessage,
                        PortfolioResponseMessageDto>(
                        InformaticaHelper.PortfolioSnapshotTopicName,
                        new MessageBase(),
                        responseCallback,
                        failureCallbackInfo => { });

            CommandManager.AddCommand(requestCmd);
        }

        public void GetPriceTimestamps(
            DateTime priceDate,
            Action<PriceTimestampsResponseMessage> responseCallback,
            Action<FailureCallbackInfo> failureCallback)
        {
            SendRequestCommand<PriceTimestampsRequestMessage, PriceTimestampsResponseMessage> requestCmd =
                new SendRequestCommand<PriceTimestampsRequestMessage, PriceTimestampsResponseMessage>(
                    InformaticaHelper.PriceTimestampsTopicName,
                    new PriceTimestampsRequestMessage { PriceDate = priceDate },
                    responseCallback,
                    failureCallback);

            CommandManager.AddCommand(requestCmd);
        }

        public void GetHistoricalPnlSnapshot(
            DateTime sourceDataDate,
            DateTime riskDate,
            DateTime priceDate,
            SourceDataType dataType,
            string accountNumber,
            string exchangeValue,
            List<ProductPriceDetail> externalPrices,
            Action<List<HistoricalPnlSnapshotMessage>> responseCallback)
        {
            HistoricalPnlSnapshotMessage message = new HistoricalPnlSnapshotMessage
            {
                SourceDataDate = sourceDataDate,
                RiskDate = riskDate,
                PriceDate = priceDate,
                DataType = dataType,
                AccountNumber = accountNumber,
                ExchangeValue = exchangeValue,
                ExternalPrices = externalPrices
            };

            GetSnapshot<HistoricalPnlSnapshotMessage, HistoricalPnlSnapshotMessageDto>(
                InformaticaHelper.HistoricalPnlSnapshotTopicName,
                message,
                responseCallback);
        }

        //Todo change to asyn/observable or something
        [Obsolete]
        public void CalculateSpreader(
            List<SpreaderInput> manualInput,
            int productId,
            DateTime startDate,
            DateTime endDate,
            Action<SpreaderResponseMessage> responseCallback)
        {
            SpreaderRequestMessage message = new SpreaderRequestMessage
            {
                ManualInput = manualInput,
                ProductId = productId,
                StartDate = startDate,
                EndDate = endDate,
            };

            CommandManager.AddCommand(new SpreaderCalculationCommand(message, responseCallback));
        }

        public void CalculateSpreader(
            List<SpreaderInput> manualInput,
            List<SpreaderInput> positionsInput,
            int productId,
            DateTime startDate,
            DateTime endDate,
            bool includeSimulatedPositions,
            Action<SpreaderResponseMessage> responseCallback)
        {
            SpreaderRequestMessage message = new SpreaderRequestMessage
            {
                ManualInput = manualInput,
                PositionsInput = positionsInput,
                ProductId = productId,
                StartDate = startDate,
                EndDate = endDate,
                IncludeSimulatedPositions = includeSimulatedPositions
            };

            CommandManager.AddCommand(new SpreaderCalculationCommand(message, responseCallback));
        }

        public void EditPortfolio(
            Portfolio portfolio,
            MessageStatusCode statusCode,
            Action<PortfolioEditMessage> responseCallback,
            Action<FailureCallbackInfo> failureCallback = null)
        {
            PortfolioEditMessage message = new PortfolioEditMessage
            {
                Portfolio = portfolio,
                UserName = GetUserData().UserName,
                StatusCode = statusCode,
            };

            PortfolioEditMessageDto dto = Mapper.Map<PortfolioEditMessage, PortfolioEditMessageDto>(message);

            CommandManager.AddCommand(new PortfolioEditCommand(dto, responseCallback, failureCallback));
        }

        public void GetSpreaderProducts(
            Action<SpreaderProductsResponseMessage> responseCallback,
            Action<FailureCallbackInfo> onRequestFail)
        {
            SpreaderProductsMessage message = new SpreaderProductsMessage();

            CommandManager.AddCommand(new SpreaderProductsCommand(message, responseCallback, onRequestFail));
        }

        public void GetPositionsSnapshot(
            DateTime? dailyDate,
            DateTime? riskDate,
            Action<List<PositionsSnapshotMessage>> responseCallback,
            bool excludeTasTrades = true)
        {
            PositionsSnapshotMessage message = new PositionsSnapshotMessage
            {
                DailyDate = dailyDate,
                RiskDate = riskDate,
                ExcludeTasTrades = excludeTasTrades
            };

            GetPositionsSnapshot(message, responseCallback);
        }

        private void GetPositionsSnapshot(
            PositionsSnapshotMessage message,
            Action<List<PositionsSnapshotMessage>> responseCallback)
        {
            GetSnapshot<PositionsSnapshotMessage, PositionsSnapshotMessageDto>(
                InformaticaHelper.PositionsSnapshotTopicName,
                message,
                responseCallback);
        }

        public void GetPositionsWithTradesSnapshot(
            DateTime? dailyDate,
            DateTime? riskDate,
            Action<List<PositionsWithTradesSnapshotMessage>> responseCallback,
            bool excludeTasTrades = true)
        {
            PositionsWithTradesSnapshotMessage message = new PositionsWithTradesSnapshotMessage
            {
                DailyDate = dailyDate,
                RiskDate = riskDate,
                ExcludeTasTrades = excludeTasTrades
            };

            GetSnapshot<PositionsWithTradesSnapshotMessage, PositionsWithTradesSnapshotMessageDto>(
                InformaticaHelper.PositionsWithTradesSnapshotTopicName,
                message,
                responseCallback);
        }

        public void GetTasSnapshot(
            DateTime? startDate,
            DateTime? endDate,
            DateTime? positionDate,
            int? portfolioId,
            bool includeFutures,
            Action<List<TasSnapshotMessage>> responseCallback)
        {
            TasSnapshotMessage message = new TasSnapshotMessage
            {
                StartDate = startDate,
                EndDate = endDate,
                PositionDate = positionDate,
                PortfolioId = portfolioId,
                IncludeFutures = includeFutures
            };

            GetSnapshot<TasSnapshotMessage, TasSnapshotMessageDto>(
                InformaticaHelper.TasSnapshotTopicName,
                message,
                responseCallback);
        }

        public void GetOvernightPnlSnapshot(
            DateTime? priceDate,
            bool isLiveMode,
            int? portfolioId,
            DateTime? livePriceSnapshotDatetime,
            string currency,
            Action<List<OvernightPnlSnapshotMessage>> responseCallback)
        {
            OvernightPnlSnapshotMessage message = new OvernightPnlSnapshotMessage
            {
                PriceDate = priceDate,
                IsLiveMode = isLiveMode,
                PortfolioId = portfolioId,
                LivePriceSnapshotDatetime = livePriceSnapshotDatetime,
                Currency = currency
            };

            GetSnapshot(InformaticaHelper.OvernightPnlSnapshotTopicName, message, responseCallback);
        }

        public void GetPnLByLegsSnapshot(
            DateTime? priceDate,
            bool isLiveMode,
            int? portfolioId,
            DateTime? livePriceSnapshotDatetime,
            string currency,
            Action<List<PnLByProductSnapshotMessage>> responseCallback)
        {
            PnLByProductSnapshotMessage message = new PnLByProductSnapshotMessage()
            {
                PriceDate = priceDate,
                IsLiveMode = isLiveMode,
                PortfolioId = portfolioId,
                LivePriceSnapshotDatetime = livePriceSnapshotDatetime,
                Currency = currency
            };

            GetSnapshot(InformaticaHelper.PnLByProductSnapshotTopicName, message, responseCallback);
        }

        public void GetLiveFeedSnapshot(
            string topicName,
            List<int> sequences,
            Action<List<LiveFeedReplaySnapshotMessage>> responseCallback)
        {
            LiveFeedReplaySnapshotMessage message = new LiveFeedReplaySnapshotMessage()
            {
                TopicName = topicName,
                Sequences = sequences
            };

            GetSnapshot<LiveFeedReplaySnapshotMessage, LiveFeedReplaySnapshotMessageDto>(
                InformaticaHelper.LiveFeedReplayTopicName,
                message,
                responseCallback);
        }

        public void GetCumulativeSnapshot(PnlCumulativeArgs args, Action<byte[]> responseCallback)
        {
            CommandManager.AddCommand(new RequestCumulativeCommand(args, responseCallback)); //VV: Cumulative
        }

        public void GetPermissionSnapshot(
            PrivilegesArgs args,
            Action<UserDataMessage> responseCallback,
            Action<FailureCallbackInfo> timeoutCallback = null)
        {
            SendRequest(InformaticaHelper.UserPermissionSnapshotTopicName, args, responseCallback, 5000, timeoutCallback);
        }

        public void GetPnlHistoricalDateSnapshot(
            PnlHistoricalDateMessageArgs args,
            Action<PnlHistoricalDateMessage> responseCallback)
        {
            CommandManager.AddCommand(new PnlHistoricalDateCommand(args, responseCallback)); //VV: Pnl record
        }

        public void GetPnlHistoricalInformationSnapshot(
            PnlHistoricalInformationMessageArgs args,
            Action<PnlHistoricalInformationMessage> responseCallback)
        {
            CommandManager.AddCommand(new PnlHistoricalInformationCommand(args, responseCallback)); //VV: Pnl record
        }

        public void GetPnlHistoricalSaveSnapshot(
            PnlHistoricalSaveMessageArgs args,
            Action<PnlHistoricalSaveMessage> responseCallback)
        {
            CommandManager.AddCommand(new PnlHistoricalSaveCommand(args, responseCallback)); //VV: Pnl record
        }

        public void GetMatchingDummiesSnapshot(
            MatchingDummiesMessageArgs args,
            Action<MatchingDummiesSnapshotMessage> responseCallback)
        {
            CommandManager.AddCommand(new MatchingDummiesSnapshotCommand(args, responseCallback));
            //VV: Get MatchingDummies snapshot message
        }

        public void SetWrongMatchingDummies(MatchingDummiesWrongMessageArgs args)
        {
            CommandManager.AddCommand(new MatchingDummiesWrongCommand(args)); //VV: Wrong MatchingDummies record
        }

        public void OnSequenceReset()
        {
            _privileges.GetPermissionsFromServer();

            RefreshLivePositions();

            if (SequenceReset != null)
            {
                SequenceReset(this, new EventArgs());
            }
        }

        private void RefreshLivePositions()
        {
            PositionsReceived = false;
            Positions.Clear();

            PositionsSnapshotMessage message = new PositionsSnapshotMessage { ExcludeTasTrades = true };

            GetPositionsSnapshot(message, RefreshLivePositions_Callback);
        }

        private void RefreshLivePositions_Callback(List<PositionsSnapshotMessage> positionSnapshots)
        {
            if ((positionSnapshots == null) || (positionSnapshots.Count == 0))
            {
                PositionsReceived = true;
                return;
            }

            List<CalculationDetail> positions = positionSnapshots
                                                .Where(snapshot => snapshot.Positions != null)
                                                .SelectMany(snapshot => snapshot.Positions)
                                                .ToList();
            //int lastSequenceNumber = messages.Max(x => x.LastSequenceNumber);

            foreach (CalculationDetail position in positions)
            {
                if (!Positions.TryAdd(position.GetKey(), position))
                {
                    // todo: log waring
                }
            }

            PositionsReceived = true;
            PositionChanged?.Invoke(this, new EventArgs());
            //PendingPositionsUpdates.ResumeDequeuingFrom(lastSequenceNumber);
        }

        public void ExpiringProductsMarkAsDelivered(int securityDefinitionId)
        {
            CommandManager.AddCommand(new ExpiringProductsMarkAsDeliveredCommand(securityDefinitionId));
        }

        public void AssignTrades(
            int toPortfolioId,
            string toPortfolioName,
            Dictionary<string, Tuple<List<DateTime?>, List<string>>> execIdsOfTradesToAssign,
            Action<AssignTradeResponseMessage> responseCallback,
            Action<FailureCallbackInfo> timeoutCallback)
        {
            AssignTradeRequestMessage requestMessage = new AssignTradeRequestMessage(
                toPortfolioId,
                toPortfolioName,
                execIdsOfTradesToAssign);

            var requestCommand =
                new SendRequestCommand<AssignTradeRequestMessage, AssignTradeResponseMessage>(
                    InformaticaHelper.AssignTradesTopicName,
                    requestMessage,
                    responseCallback,
                    timeoutCallback);

            CommandManager.AddCommand(requestCommand);
        }

        public void UpdatePortfolios(List<Portfolio> portfolios)
        {
            if (portfolios != null)
            {
                Portfolios =
                    new ConcurrentDictionary<int, Portfolio>(portfolios.ToDictionary(p => p.PortfolioId, p => p));

                OnPortfoliosChanged(portfolios);
            }
        }

        private void OnPortfoliosChanged(List<Portfolio> portfolios)
        {
            if (portfolios != null)
            {
                if (PortfoliosChanged != null)
                {
                    PortfoliosChanged(this, new PortfoliosChangedEventArgs(portfolios));
                }
            }
        }

        private int _maxHistoricalRiskDays;
        private bool _isRestarting;
        private readonly ILogger _log;

        public MatchingDummiesObjectCollection MatchingDummies { get; private set; }

        public void UpdateMatchingDummies(MatchingDummiesObjectCollection matchingDummies)
        {
            if (null == MatchingDummies)
            {
                MatchingDummies = new MatchingDummiesObjectCollection();
            }
            else
            {
                MatchingDummies.Clear();
            }

            if (matchingDummies != null)
            {
                MatchingDummies.AddRange(matchingDummies);

                OnMatchingDummiesChanged(MatchingDummies);
            }
        }

        public void OnVarLatestUpdated(VarLatestUpdateHandler.VarDataEventArgs e)
        {
            EventHandler<VarLatestUpdateHandler.VarDataEventArgs> handler = VarLatestUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnPnlDiffUpdated(PnlDiffUpdateHandler.PnlDiffEventArgs args)
        {
            EventHandler<PnlDiffUpdateHandler.PnlDiffEventArgs> handler = PnlDiffUpdated;

            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void OnMatchingDummiesChanged(MatchingDummiesObjectCollection matchingDummies)
        {
            if (matchingDummies != null)
            {
                if (MatchingDummiesChanged != null)
                {
                    MatchingDummiesChanged(this, new MatchingDummiesChangedEventArgs(matchingDummies));
                }
            }
        }

        private void Match_Callback(MatchingDummiesSnapshotMessage matchingDummiesSnapshotMessage)
        {
            if ((matchingDummiesSnapshotMessage != null)
                && (matchingDummiesSnapshotMessage.MatchingDummiesObjectCollection != null))
            {
                UpdateMatchingDummies(matchingDummiesSnapshotMessage.MatchingDummiesObjectCollection);
            }
        }

        public void CancelTrades(
            int userId,
            List<int> cancelTradeIds,
            string editCancelReason,
            Action<TradesEditResponseMessage> responseCallback,
            Action<FailureCallbackInfo> responseCallbackOnFailure)
        {
            TradesCancelMessage message = new TradesCancelMessage
            {
                UserId = userId,
                CancelTradeIds = cancelTradeIds,
                EditCancelReason = editCancelReason
            };

            CommandManager.AddCommand(
                new SendRequestCommand<TradesCancelMessage, TradesEditResponseMessage>(
                    InformaticaHelper.CancelTradesTopicName,
                    message,
                    responseCallback,
                    5000,
                    responseCallbackOnFailure));
        }

        public void UncancelTrade(
            int userId,
            string username,
            int tradeId,
            string uncancelReason,
            Action<TradesEditResponseMessage> responseCallback,
            Action<FailureCallbackInfo> responseCallbackOnFailture)
        {
            TradesUncancelMessage message = new TradesUncancelMessage
            {
                UserId = userId,
                Username = username,
                UncancelTradeId = tradeId,
                UncancelReason = uncancelReason
            };

            CommandManager.AddCommand(
                new SendRequestCommand<TradesUncancelMessage, TradesEditResponseMessage>(
                    InformaticaHelper.UncancelTradesTopicName,
                    message,
                    responseCallback,
                    5000,
                    responseCallbackOnFailture));
        }

        public void EditTransferTrades(
            int userId,
            string username,
            int? editTradeId,
            int? portfolioId1,
            int? portfolioId2,
            List<TradeCapture> newTrades,
            List<int> cancelTradeIds,
            string editCancelReason,
            Action<TradesEditResponseMessage> responseCallback,
            Action<FailureCallbackInfo> responseCallbackOnFailture)
        {
            TradesEditMessage message = new TradesEditMessage
            {
                UserId = userId,
                Username = username,
                EditTradeId = editTradeId,
                Portfolio1 = portfolioId1,
                Portfolio2 = portfolioId2,
                CancelTradeIds = cancelTradeIds,
                EditCancelReason = editCancelReason,
                NewTradeCaptures = newTrades
            };

            CommandManager.AddCommand(
                new SendRequestDtoCommand
                    <TradesEditMessage, TradesEditMessageDto, TradesEditResponseMessage, TradesEditResponseMessage>(
                    InformaticaHelper.EditTradesTopicName,
                    message,
                    5000,
                    responseCallback,
                    responseCallbackOnFailture));
        }

        public void EditManualTrades(
            int userId,
            string username,
            int? editTradeId,
            int? portfolioId1,
            List<TradeCapture> newTrades,
            List<int> cancelTradeIds,
            string editCancelReason,
            Action<TradesEditResponseMessage> responseCallback,
            Action<FailureCallbackInfo> responseCallbackOnFailture)
        {
            TradesEditMessage message = new TradesEditMessage
            {
                UserId = userId,
                Username = username,
                EditTradeId = editTradeId,
                Portfolio1 = portfolioId1,
                CancelTradeIds = cancelTradeIds,
                EditCancelReason = editCancelReason,
                NewTradeCaptures = newTrades
            };

            CommandManager.AddCommand(
                new SendRequestDtoCommand
                    <TradesEditMessage, TradesEditMessageDto, TradesEditResponseMessage, TradesEditResponseMessage>(
                    InformaticaHelper.EditTradesTopicName,
                    message,
                    5000,
                    responseCallback,
                    responseCallbackOnFailture));
        }

        public void CalculateTradesImpact(
            List<TradeCapture> tradeCaptures,
            Action<TradeImpactResponseMessage> responseCallback,
            Action<FailureCallbackInfo> timeoutCallback)
        {
            TradeImpactRequestMessage message = new TradeImpactRequestMessage() { TradeCaptures = tradeCaptures };

            CommandManager.AddCommand(
                new SendRequestDtoCommand
                    <TradeImpactRequestMessage, TradeImpactRequestMessageDto, TradeImpactResponseMessage,
                        TradeImpactResponseMessageDto>(
                    InformaticaHelper.TradeImpactTopicName,
                    message,
                    5000,
                    responseCallback,
                    timeoutCallback));
        }

        public void GetProductCategories(
            Action<ProductCategoryDtoMessage> responseCallback,
            Action<FailureCallbackInfo> timeoutCallback)
        {
            SendRequest<ProductCategoryMessage, ProductCategoryDtoMessage>(
                InformaticaHelper.ProductCategoryInfoTopicName,
                new ProductCategoryMessage() { Categories = new List<ProductCategory>() },
                responseCallback,
                5000,
                timeoutCallback);
        }

        public void GetProductAliases(
            Action<EditTradeParamsMessage> responseCallback,
            Action<FailureCallbackInfo> timeoutCallback)
        {
            EditTradeParamsMessage message = new EditTradeParamsMessage();

            CommandManager.AddCommand(
                new SendRequestDtoCommand
                    <EditTradeParamsMessage, EditTradeParamsMessageDto, EditTradeParamsMessage,
                        EditTradeParamsMessageDto>(
                    InformaticaHelper.EditTradeParamsTopicName,
                    message,
                    responseCallback,
                    timeoutCallback));
        }

        public void GetOfficialProductNames(
            OfficialProductNameMessage request,
            Action<OfficialProductNameMessage> responseCallback,
            Action<FailureCallbackInfo> timeoutCallback)
        {
            CommandManager.AddCommand(
                new SendRequestCommand<OfficialProductNameMessage, OfficialProductNameMessage>(
                    InformaticaHelper.OfficialProductInfoTopicName,
                    request,
                    responseCallback,
                    5000,
                    timeoutCallback));
        }

        public void ValidateBalmoDate(
            ValidateBalmoDateRequest request,
            Action<ValidateBalmoDateResponse> responseCallback,
            Action<FailureCallbackInfo> timeoutCallback)
        {
            CommandManager.AddCommand(
                new SendRequestCommand<ValidateBalmoDateRequest, ValidateBalmoDateResponse>(
                    InformaticaHelper.ValidateBalmoDateTopicName,
                    request,
                    responseCallback,
                    10000,
                    timeoutCallback));
        }

        public void SendRequest<TRequest, TResponse>(
            string topicName,
            TRequest requestMessage,
            Action<TResponse> responseCallback,
            Action<FailureCallbackInfo> timeoutCallback) where TRequest : MessageBase where TResponse : MessageBase
        {
            CommandManager.AddCommand(
                new SendRequestCommand<TRequest, TResponse>(topicName, requestMessage, responseCallback, timeoutCallback));
        }

        public void SendMessage<TMessage>(string topicName, TMessage message) where TMessage : MessageBase
        {
            CommandManager.AddCommand(new SendMessageCommand<TMessage>(topicName, message));
        }

        public void SendRequest<TRequest, TResponse>(
            string topicName,
            TRequest requestMessage,
            Action<TResponse> responseCallback,
            int responseTimeout,
            Action<FailureCallbackInfo> responseCallbackOnFailure) where TRequest : MessageBase where TResponse : MessageBase
        {
            CommandManager.AddCommand(
                new SendRequestCommand<TRequest, TResponse>(
                    topicName,
                    requestMessage,
                    responseCallback,
                    responseTimeout,
                    responseCallbackOnFailure));
        }

        public void SendRequest<TRequest, TRequestDto, TResponse, TResponseDto>(
            string topicName,
            TRequest requestMessage,
            Action<TResponse> responseCallback,
            int responseTimeout,
            Action<FailureCallbackInfo> responseCallbackOnFailure) where TRequest : MessageBase where TRequestDto : MessageBase
            where TResponse : MessageBase where TResponseDto : MessageBase
        {
            CommandManager.AddCommand(
                new SendRequestDtoCommand<TRequest, TRequestDto, TResponse, TResponseDto>(
                    topicName,
                    requestMessage,
                    responseTimeout,
                    responseCallback,
                    responseCallbackOnFailure));
        }

        public void WriteAudit(
            WriteAuditRequestMessage message,
            Action<WriteAuditResponseMessage> responseCallback,
            Action<FailureCallbackInfo> failtureCallback)
        {
            SendRequest(InformaticaHelper.WriteAuditTopicName, message, responseCallback, 5000, failtureCallback);
        }

        public void ChangePassword(
            string username,
            string passwordHash,
            string newPasswordHash,
            Action<ChangePasswordResponse> responseCallback,
            Action<FailureCallbackInfo> timeoutCallback)
        {
            ChangePasswordRequest request = new ChangePasswordRequest
            {
                UserName = username,
                PasswordHash = passwordHash,
                NewPasswordHash = newPasswordHash
            };
            SendRequest(InformaticaHelper.ChangePasswordTopicName, request, responseCallback, 5000, timeoutCallback);
        }

        public void CalculateDerisking(
            DeriskingCalcSnapshotMessage message,
            Action<List<DeriskingCalcSnapshotMessage>> responseCallback)
        {
            GetSnapshot<DeriskingCalcSnapshotMessage>(InformaticaHelper.DeriskingCalcTopicName, message, responseCallback);
        }

        public void OnRolloff(List<RolloffDetail> rolloffDetails)
        {
            if (RolloffNotification != null)
            {
                RolloffDetails = rolloffDetails;

                RolloffNotificationEventArgs args = new RolloffNotificationEventArgs()
                {
                    RolloffDetails = rolloffDetails
                };

                RolloffNotification(this, args);
            }
        }

        public void OnTasCheckUpdate(List<TasCheckDetail> tasCheckDetails)
        {
            tasCheckDetails = GetFilteredByPortfolioPermissionsTasCheckDetail(tasCheckDetails);

            try
            {
                LockTasErrors.EnterWriteLock();

                if (tasCheckDetails != null)
                {
                    _currentTasErrors = new List<TasCheckDetail>(tasCheckDetails);
                }
                else
                {
                    _currentTasErrors = new List<TasCheckDetail>();
                }
            }
            finally
            {
                LockTasErrors.ExitWriteLock();
            }

            TasCheckErrorsNotification?.Invoke(this, new TasCheckErrorsEventArgs(tasCheckDetails));
        }

        public void IgnoreTasCheckError(
            string key,
            Action<TasCheckSkipResponseMessage> responseCallback,
            Action<FailureCallbackInfo> timeoutCallback)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            CommandManager.AddCommand(
                new SendRequestCommand<TasCheckSkipRequestMessage, TasCheckSkipResponseMessage>(
                    InformaticaHelper.TasCheckSkipTopicName,
                    new TasCheckSkipRequestMessage { KeyToSkip = key },
                    responseCallback,
                    timeoutCallback));
        }

        public static bool IsCurrentUserCanSeeErrorForUser(string userName)
        {
            UserData userData = Instance.GetUserData();

            if ((Instance == null) || (userData == null) || string.IsNullOrEmpty(userName))
            {
                return true;
            }

            return Instance.IsCurrentUserAuthorized(
                       new List<PermissionType>() { PermissionType.Administrator, PermissionType.SuperAdministrator })
                   || (userData.UserName == userName);
        }

        private bool IsCurrentUserAuthorized(List<PermissionType> permissions)
        {
            return permissions.Any(perm => _privileges.IsCurrentUserAuthorizedTo(perm));
        }

        public static bool IsCurrentUserCanSeeAdmAlert(string userName)
        {
            UserData userData = Instance?.GetUserData();

            if ((Instance == null) || (userData == null) || string.IsNullOrEmpty(userName))
            {
                return false;
            }

            return userName.Equals(userData.UserName, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsCurrentUserAuthorizedToUseMasterTool()
        {
            UserData userData = Instance?.GetUserData();

            if ((Instance == null) || (userData == null))
            {
                return false;
            }

            return userData.PortfolioPermissions.Any(p => p.CanUseMasterTool);
        }

        /// <summary>Get trade support snapshot.All server has</summary>
        /// <param name="responseCallback">Callback to process</param>
        public void GetTradeSupportSnapshot(Action<List<TradeSupportMessage>> responseCallback)
        {
            TradeSupportMessage message = new TradeSupportMessage() { DatesOffset = TradeSupportLogPeriod };
            GetSnapshot(InformaticaHelper.TradeSupportSnapshotTopicName, message, responseCallback);
        }

        public void GetTradeSupportSnapshot(int offset, Action<List<TradeSupportMessage>> responseCallback)
        {
            offset = offset > -1 ? offset : 0;
            TradeSupportMessage message = new TradeSupportMessage() { DatesOffset = offset };
            GetSnapshot(InformaticaHelper.TradeSupportSnapshotTopicName, message, responseCallback);
        }

        public void AcknowledgeTradeSupport(Guid tradeSupportAlertId)
        {
            TradeSupportAlert tradeSupportAlert;
            if (TradeSupportAlerts.TryGetValue(tradeSupportAlertId, out tradeSupportAlert))
            {
                tradeSupportAlert.IsAcknowledged = true;
                tradeSupportAlert.Details = null;
                tradeSupportAlert.IgnoreMessages = null;
                tradeSupportAlert.Acknowledge = null;

                // fire ack event
                if (TradeSupportAcknowledged != null)
                {
                    TradeSupportAcknowledged(this, new TradeSupportEventArgs { TradeSupportAlert = tradeSupportAlert });
                }
            }
        }

        private List<TasCheckDetail> GetFilteredByPortfolioPermissionsTasCheckDetail(List<TasCheckDetail> tasCheckDetails)
        {
            return tasCheckDetails.Where(
                                      tasCheck => tasCheck.PortfolioIds != null
                                                  && tasCheck.PortfolioIds.Any(
                                                      portfolio => _privileges
                                                          .IsCurrentUserAuthorizedToPortfolio(
                                                              portfolio,
                                                              PortfolioPermission.CanViewRisk)))
                                  .ToList();
        }

        public void RegisterTradeSupportAlertFilter(string name, Predicate<TradeSupportAlert> predicate)
        {
            _tradeSupportFilters.TryAdd(name, predicate);
        }

        public void UnregisterTradeSupportAlertFilter(string name)
        {
            Predicate<TradeSupportAlert> _;

            _tradeSupportFilters.TryRemove(name, out _);
        }

        public void AddTradeSupport(TradeSupportAlert tradeSupportAlert)
        {
            if (!CanHandleTradeSupportAlert(tradeSupportAlert))
            {
                return;
            }

            TradeSupportAlerts.TryAdd(tradeSupportAlert.Id, tradeSupportAlert);

            // fire add event
            if (TradeSupportAdded != null)
            {
                TradeSupportAdded(this, new TradeSupportEventArgs { TradeSupportAlert = tradeSupportAlert });
            }
        }

        private bool CanHandleTradeSupportAlert(TradeSupportAlert tradeSupportAlert)
        {
            IEnumerable<Predicate<TradeSupportAlert>> filters = _tradeSupportFilters.Values;

            return tradeSupportAlert != null
                && HasPermissionToHandleTradeSupportAlert(tradeSupportAlert)
                && (!filters.Any() || filters.All(allow => allow(tradeSupportAlert)));
        }

        private bool HasPermissionToHandleTradeSupportAlert(TradeSupportAlert tradeSupportAlert)
        {
            if (null == tradeSupportAlert.Parameters)
            {
                // There's no parameter to check, so there's nothing to view anyway.
                return true;
            }

            Type parametersType = tradeSupportAlert.Parameters.GetType();

            if (_addTradeSupportAlertPredicates.ContainsKey(parametersType) &&
                !_addTradeSupportAlertPredicates[parametersType](tradeSupportAlert))
            {
                return false;
            }

            return true;
        }

        private bool HasPermissionForTasCheckPortfolios(TradeSupportAlert alert)
        {
            if (null == alert.Parameters)
            {
                return false;
            }

            List<TasCheckDetail> tasCheckDetail = alert.Parameters as List<TasCheckDetail>;

            tasCheckDetail = GetFilteredByPortfolioPermissionsTasCheckDetail(tasCheckDetail);
            return tasCheckDetail.Count > 0;
        }

        public void GetTradeAddPrerequisities(
            int tradeCaptureId,
            bool isDuplicateMode,
            bool isMasterToolMode,
            Action<TradeAddPrerequisitesResponseMessage> responseCallback,
            Action<FailureCallbackInfo> timeoutCallback)
        {
            TradeAddPrerequisitesRequestMessage requestMessage = new TradeAddPrerequisitesRequestMessage()
            {
                TradeCaptureId = tradeCaptureId,
                IsDuplicateMode = isDuplicateMode,
                IsMasterToolMode = isMasterToolMode,
                UserId = GetCurrentUser().UserId,
            };

            CommandManager.AddCommand(
                new SendRequestDtoCommand
                    <TradeAddPrerequisitesRequestMessage, TradeAddPrerequisitesRequestMessage,
                        TradeAddPrerequisitesResponseMessage, TradeAddPrerequisitesResponseMessageDto>(
                    InformaticaHelper.TradeAddPrerequisitesTopicName,
                    requestMessage,
                    responseCallback,
                    timeoutCallback));
        }

        public User GetCurrentUser()
        {
            UserData userData = GetUserData();

            if (userData != null)
            {
                Portfolio portfolio = GetUserPortfolio(userData);

                return new User { UserId = userData.UserId, UserName = userData.UserName, Portfolio = portfolio };
            }

            return null;
        }

        private static Portfolio GetUserPortfolio(UserData userData)
        {
            Portfolio portfolio = null;

            if (userData.Portfolio != null)
            {
                portfolio = new Portfolio
                {
                    PortfolioId = userData.Portfolio.PortfolioId,
                    Name = userData.Portfolio.Name
                };
            }

            return portfolio;
        }

        public void GetTradeAddImpact(
            TradeAddDetails tradeAddDetails,
            bool isMasterToolMode,
            Action<TradeAddImpactResponseMessage> responseCallback,
            Action<FailureCallbackInfo> timeoutCallback)
        {
            TradeAddImpactRequestMessage requestMessage = new TradeAddImpactRequestMessage
            {
                TradeAddDetails = tradeAddDetails,
                IsMasterToolMode = isMasterToolMode
            };

            CommandManager.AddCommand(
                new SendRequestDtoCommand
                    <TradeAddImpactRequestMessage, TradeAddImpactRequestMessageDto, TradeAddImpactResponseMessage,
                        TradeAddImpactResponseMessageDto>(
                    InformaticaHelper.TradeAddImpactTopicName,
                    requestMessage,
                    responseCallback,
                    timeoutCallback));
        }

        public void GetTradeAddCreate(
            TradeAddDetails tradeAddDetails,
            bool isMasterToolMode,
            Action<TradeAddCreateResponseMessage> responseCallback,
            Action<FailureCallbackInfo> timeoutCallback)
        {
            TradeAddCreateRequestMessage requestMessage = new TradeAddCreateRequestMessage
            {
                TradeAddDetails = tradeAddDetails,
                IsMasterToolMode = isMasterToolMode
            };
            _log.Info("Adding trade with guid {0}", requestMessage.RequestId);
            CommandManager.AddCommand(
                new SendRequestDtoCommand
                    <TradeAddCreateRequestMessage, TradeAddCreateRequestMessageDto, TradeAddCreateResponseMessage,
                        TradeAddCreateResponseMessageDto>(
                    InformaticaHelper.TradeAddCreateTopicName,
                    requestMessage,
                    responseCallback,
                    timeoutCallback));
        }

        public void RestartBusClient(string serverPrefix)
        {
            if (_isRestarting)
            {
                return;
            }

            _isRestarting = true;
            _log.Info("Restarting bus client. New server prefix: {0}", serverPrefix);

            try
            {
                Stop();
                InformaticaHelper.ServerPrefixes = new[] { serverPrefix };
                Start();
            }
            finally
            {
                _isRestarting = false;
            }
        }

        public void ChangeSnapshotTime(
            DateTime snapshotTime,
            Action<ChangeSnapshotTimeResponseMessage> responseCallback,
            Action<FailureCallbackInfo> failureCallback)
        {
            ChangeSnapshotTimeRequestMessage requestMessage =
                new ChangeSnapshotTimeRequestMessage { SnapshotTime = snapshotTime };

            CommandManager.AddCommand(
                new SendRequestCommand<ChangeSnapshotTimeRequestMessage, ChangeSnapshotTimeResponseMessage>(
                    InformaticaHelper.ChangeSnapshotTimeTopicName,
                    requestMessage,
                    responseCallback,
                    5000,
                    failureCallback));
        }

        public void TriggerServerReconnection(string prefix)
        {
            _log.Info("Received external reconnection request");

            if (ServerReconnectionNeeded != null)
            {
                _log.Info("Firing reconnection command");
                ServerReconnectionNeeded(this, new ServerReconnectionEventArgs { Prefix = prefix });
            }
            else
            {
                _log.Info(
                    "External reconnection request will not be processed because this application does not support reconnection");
            }
        }
        
        public void OnConnectionLost()
        {
            _log.Info("Firing Connection Lost event");
            ServerConnectionLost?.Invoke(this, new EventArgs());
        }

        public void GetLiveTradesPnl(DateTime priceDate, Action<List<LiveTradesPnlSnapshotMessage>> responseCallback)
        {
            LiveTradesPnlSnapshotMessage message = new LiveTradesPnlSnapshotMessage() { SnapshotDatetime = priceDate };

            GetSnapshot<LiveTradesPnlSnapshotMessage>(InformaticaHelper.LiveTradesPnlTopicName, message, responseCallback);
        }

        public void AmendBrokerage(
            int tradeCaptureId,
            decimal brokerage,
            Action<AmendBrokerageResponseMessage> responseCallback,
            Action<FailureCallbackInfo> timeoutCallback)
        {
            AmendBrokerageRequestMessage requestMessage = new AmendBrokerageRequestMessage
            {
                TradeCaptureId = tradeCaptureId,
                Brokerage = brokerage,
                UserName = GetUserData().UserName
            };

            CommandManager.AddCommand(
                new SendRequestCommand<AmendBrokerageRequestMessage, AmendBrokerageResponseMessage>(
                    InformaticaHelper.AmendBrokerageTopicName,
                    requestMessage,
                    responseCallback,
                    5000,
                    timeoutCallback));
        }

        public void GetTradesPnlOnDate(
            DateTime livePriceSnapshotDatetime,
            DateTime filterFrom,
            DateTime filterTo,
            Action<List<TradesPnlOnDateSnapshotMessage>> responseCallback)
        {
            TradesPnlOnDateSnapshotMessage message = new TradesPnlOnDateSnapshotMessage()
            {
                LivePriceSnapshotDatetime = livePriceSnapshotDatetime,
                FilterFrom = filterFrom,
                FilterTo = filterTo,
            };

            GetSnapshot(InformaticaHelper.TradesPnlOnDateTopicName, message, responseCallback);
        }

        public void ExportSourceData(
            DateTime dateTime,
            int dataType,
            DateTime snapshotDatetime,
            Action<List<ExportSourceDataSnapshotMessage>> responseCallback)
        {
            ExportSourceDataSnapshotMessage message = new ExportSourceDataSnapshotMessage
            {
                Date = dateTime,
                DataType = dataType,
                SnapshotDatetime = snapshotDatetime
            };

            GetSnapshot(InformaticaHelper.ExportSourceDataTopicName, message, responseCallback);
        }

        public void SetUserData(UserData userData)
        {
            try
            {
                _lockUserData.EnterWriteLock();
                _userData = userData;
            }
            finally
            {
                _lockUserData.ExitWriteLock();
            }
        }

        public UserData GetUserData()
        {
            try
            {
                _lockUserData.EnterReadLock();
                return _userData;
            }
            finally
            {
                _lockUserData.ExitReadLock();
            }
        }

        public void GetTradesForPricingReport(
            IEnumerable<Cell> cells,
            Portfolio portfolio,
            Action<List<PricingReportTradesSnapshotMessage>> responseCallback)
        {
            PricingReportTradesSnapshotMessage message = new PricingReportTradesSnapshotMessage
            {
                Cells = cells.ToList(),
                PortfolioId = portfolio.PortfolioId
            };

            GetSnapshot(InformaticaHelper.PricingReportTradesTopicName, message, responseCallback);
        }

        public void GetDailyReconciliationSnapshot(
            DateTime reportDate,
            DateTime riskDate,
            SourceDataType sourceDataType,
            Action<List<DailyReconciliationSnapshotMessage>> responseCallback)
        {
            DailyReconciliationSnapshotMessage requestMessage = new DailyReconciliationSnapshotMessage
            {
                DataType = sourceDataType,
                RiskDate = riskDate,
                //DailyDate = reportDate,
                SourceDate = reportDate
            };

            GetSnapshot<DailyReconciliationSnapshotMessage, DailyReconciliationSnapshotMessageDto>(
                InformaticaHelper.DailyReconciliationSnapshotTopicName,
                requestMessage,
                responseCallback);
        }

        public void ForcePositionRecalculation()
        {
            ForceRecalculatePositionsMessage requestMessage = new ForceRecalculatePositionsMessage();

            SendMessage(InformaticaHelper.ForceRecalculatePositionsTopicName, requestMessage);
        }

        public void GetFxDetails(
            int portfolioId,
            int currencyId,
            DateTime calculationDate,
            Action<List<FxExposureMessage>> responseCallback,
            Action timeoutCallback)
        {
            FxExposureMessage requestMessage = new FxExposureMessage(
                portfolioId,
                currencyId,
                calculationDate);

            GetSnapshot(
                InformaticaHelper.FxExposureSnapshotTopicName,
                requestMessage,
                responseCallback,
                timeoutCallback);
        }

        public void GetFxHedgeDetails(
            int portfolioId,
            int currencyId,
            DateTime startDate,
            DateTime endDate,
            Action<List<FxHedgeDetailMessage>> responseCallback)
        {
            FxHedgeDetailMessage requestMessage = new FxHedgeDetailMessage(
                portfolioId,
                currencyId,
                startDate,
                endDate);

            GetSnapshot(
                InformaticaHelper.FxHedgeDetailSnapshotTopicName,
                requestMessage,
                responseCallback);
        }

        public void RaisePositionChanged()
        {
            PositionChanged?.Invoke(this, new EventArgs());
        }

        public static void SerialiseDatesToUnspecifiedKind()
        {
            JsonHelper.WithTimeZoneHandling(DateTimeZoneHandling.Unspecified);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _lockPnl?.Dispose();
                _serviceManager?.Dispose();
                _lockLiveData?.Dispose();
                _lockUserData?.Dispose();
                _privileges?.Dispose();
                LbmContext?.Dispose();

                GC.SuppressFinalize(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }

    public class ServerReconnectionEventArgs : EventArgs
    {
        public string Prefix { get; set; }
    }

    public class TradeSupportEventArgs : EventArgs
    {
        public TradeSupportAlert TradeSupportAlert { get; set; }
    }

    public class RolloffNotificationEventArgs : EventArgs
    {
        public List<RolloffDetail> RolloffDetails { get; set; }
    }

    public class MatchingDummiesChangedEventArgs : EventArgs
    {
        public MatchingDummiesObjectCollection MatchingDummiesObjectCollection { get; private set; }

        public MatchingDummiesChangedEventArgs(MatchingDummiesObjectCollection collection)
        {
            MatchingDummiesObjectCollection = collection;
        }
    }

    public enum EventType
    {
        Snapshot,
        Update,
        Failed
    }
}