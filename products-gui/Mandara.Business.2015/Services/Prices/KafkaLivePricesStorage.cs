using KafkaMessaging;
using KafkaMessaging.Consumer;
using Mandara.Business.Contracts;
using Mandara.Business.DataInterface;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Extensions.Nullable;
using Mandara.Extensions.Option;
using Mandara.IRM.Server.Services;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Mandara.Business.Services.Prices
{
    public class KafkaLivePricesStorage : PricesStorage, ILivePricesStorage
    {
        private static readonly ILogger Logger = new NLogLoggerFactory().GetCurrentClassLogger();
        private readonly LivePricesTable _livePricesTable;
        private KafkaConsumer _priceReceiver;
        private bool _consumerStarted = false;
        private const string LivePriceConsumerName = "livePrices";
        private const string EventHubEndpointAppKey = "EventHub_Endpoint";
        private const string EventHubConnectionStringAppKey = "EventHub";
        private const string EventHubTopicAppKey = "EventHub_Topic";

        public KafkaLivePricesStorage(
            IDailyPricesTable dailyPricesTable,
            IPricesTimestampsRepository pricesTimestampsRepository,
            IProductsStorage productsStorage,
            List<TimeSpan> todayTimestamps) : this(
            new LivePricesTable(null),
            dailyPricesTable,
            pricesTimestampsRepository,
            productsStorage,
            todayTimestamps)
        {
        }

        public KafkaLivePricesStorage(
            LivePricesTable livePricesTable,
            IDailyPricesTable dailyPricesTable,
            IPricesTimestampsRepository pricesTimestampsRepository,
            IProductsStorage productsStorage,
            List<TimeSpan> todayTimestamps)
            : base(dailyPricesTable, pricesTimestampsRepository, productsStorage, todayTimestamps)
        {
            _livePricesTable = livePricesTable;
            PrepareKafkaConsumer();
        }

        private void PrepareKafkaConsumer()
        {
            string eventHubEndpoint = ConfigurationManager.AppSettings[EventHubEndpointAppKey] ?? "";
            if (string.IsNullOrEmpty(eventHubEndpoint))
            {
                _priceReceiver = KafkaConsumerFactory.CreateConsumer(
                    LivePriceConsumerName,
                    OnKafkaConnectionStateChanged,
                    OnPricesReceived);
            }
            else
            {
                string eventHubConnectionString =
                    ConfigurationManager.ConnectionStrings[EventHubConnectionStringAppKey].ConnectionString ?? "";
                string eventHubTopic =
                    ConfigurationManager.AppSettings[EventHubTopicAppKey] ?? "";

                EventHubKafkaConsumerConfig eventHubConfig = new EventHubKafkaConsumerConfig()
                {
                    EventHubEndpoint = eventHubEndpoint,
                    ConnectionString = eventHubConnectionString,
                    Topic = eventHubTopic,
                    TopicMessageType = typeof(LivePriceSnapshot),
                    Group = "IrmServer",
                    ConsumerName = LivePriceConsumerName,
                    GenerateGroup = true
                };

                _priceReceiver = KafkaConsumerFactory.CreateEventhubConsumer(
                    eventHubConfig,
                    OnKafkaConnectionStateChanged,
                    OnPricesReceived);
            }
        }

        private void OnKafkaConnectionStateChanged(object sender, KafkaConnectionStateEventArgs connState)
        {
            switch (connState.State)
            {
                case ConnectionState.Connecting:
                {
                    Logger.Info("LivePrices: Connecting to Kafka");
                }
                break;

                case ConnectionState.Connected:
                {
                    Logger.Info("LivePrices: Connected to Kafka");
                }
                break;

                case ConnectionState.Disconnected:
                {
                    Logger.Info("LivePrices: Disconnected from Kafka");
                }
                break;
            }
        }

        private void OnPricesReceived(object sender, KafkaBatchReceivedEventArgs prices)
        {
            Logger.Trace("Price Updated");
            _livePricesTable.Update(
                prices.Messages.Last(allPrices => allPrices is LivePriceSnapshot) as LivePriceSnapshot);
            _dailyPricesTable.Update(_livePricesTable.PriceColHeaders, _livePricesTable.PricesByMonth);
        }

        public TryGetResult<Money> GetProductPrice(
            int productId,
            DateTime productDate,
            ProductDateType priceDateType,
            string mappingColumn,
            int officialProductId,
            DateTime? tradeStartDate = null,
            DateTime? tradeEndDate = null)
        {
            return GetProductPrice(
                productId,
                officialProductId,
                productDate,
                priceDateType,
                new DateRange(tradeStartDate, tradeEndDate),
                mappingColumn,
                _livePricesTable,
                SystemTime.Now());
        }

        public TryGetResult<Money>[] GetProductPricesByMonth(
            int productId,
            DateTime productDate,
            ProductDateType priceDateType,
            string mappingColumn,
            int officialProductId,
            DateTime? tradeStartDate = null,
            DateTime? tradeEndDate = null)
        {
            return GetProductPricesByMonth(
                productId,
                productDate,
                priceDateType,
                mappingColumn,
                officialProductId,
                _livePricesTable,
                SystemTime.Now(),
                new DateRange(tradeStartDate, tradeEndDate));
        }

        public void Update()
        {
            if ((_priceReceiver?.Connected).False() && !_consumerStarted)
            {
                _consumerStarted = true;
                _priceReceiver?.Start();
            }
        }

        public ILivePricesStorage GetFixedLivePrices()
        {
            return new LivePricesStorage(
                _livePricesTable.ShallowCopy(),
                (IDailyPricesTable)_dailyPricesTable.ShallowCopy(),
                _pricesTimestampsRepository,
                _productsStorage,
                TodayTimestamps.ToList());
        }

        protected virtual void Dispose(bool isDiposing)
        {
            if (isDiposing)
            {
                Stop();
                GC.SuppressFinalize(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Stop()
        {
            if (_priceReceiver != null)
            {
                _priceReceiver.MessageReceived -= OnPricesReceived;
                _priceReceiver.ConnectionStateChanged -= OnKafkaConnectionStateChanged;
            }

            _priceReceiver?.Dispose();
            _priceReceiver = null;
        }
    }
}
