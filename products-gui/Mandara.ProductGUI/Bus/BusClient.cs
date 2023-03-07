using System;
using System.Configuration;
using System.IO;
using System.Threading;
using Mandara.Business;
using Mandara.Business.Bus;
using Mandara.Business.Bus.Commands;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages;
using com.latencybusters.lbm;
using Mandara.Business.Bus.Messages.Features;
using Mandara.Business.Bus.Messages.ProductBreakdown;
using Mandara.Business.Bus.Messages.TradeAdd;
using Mandara.Business.Client.Managers;
using Mandara.Business.Managers;
using Mandara.Business.TradeAdd;
using Mandara.Entities.ErrorDetails;
using Ninject.Extensions.Logging;

namespace Mandara.ProductGUI.Bus
{
    public class BusClient
    {
        public static BusClient Instance { get; private set; }


        public ProductInformaticaHelper Informatica { get; private set; }
        public CommandManager CommandManager { get; private set; }


        private String LBMConfigurationFilePath
        {
            get
            {
                return ConfigurationManager.AppSettings["LBMConfigurationFilePath"];
            }
        }


        private LBMContext _lbmContext;


        public BusClient()
        {
            Instance = this;
        }

        public void Start()
        {
            if (LBMConfigurationFilePath != null)
            {
                if (File.Exists(LBMConfigurationFilePath))
                    LBM.setConfiguration(LBMConfigurationFilePath);
            }

            _lbmContext = new LBMContext();
            Thread.Sleep(3000);

            CommandManager = new CommandManager(IoC.Kernel, IoC.Get<ILoggerFactory>().GetLogger(typeof(BusClient)));
            CommandManager.Start();

            Informatica = new ProductInformaticaHelper(_lbmContext, new HandlerManager(IoC.Kernel, IoC.Get<ILoggerFactory>().GetLogger(typeof(BusClient))));
            Informatica.CreateSources();
            Informatica.CreateReceivers();

            GetFeatures(Features_Callback, (info) => { });
        }

        private void GetFeatures(Action<FeaturesResponseMessage> callback, Action<FailureCallbackInfo> failureCallback)
        {
            CommandManager.AddCommand(
                new SendRequestCommand
                    <FeaturesRequestMessage, FeaturesResponseMessage>
                    (InformaticaHelper.FeaturesTopicName, new FeaturesRequestMessage(), callback, 5000, failureCallback));
        }

        private void Features_Callback(FeaturesResponseMessage message)
        {
            if (message == null)
                return;

            FeatureManager.Init(message.Features);
        }



        public void Stop()
        {
            try
            {
                if (CommandManager != null)
                {
                    CommandManager.Stop();
                }

                if (Informatica != null)
                {
                    Informatica.CloseSources();
                    Informatica.CloseReceivers();
                }

                if (_lbmContext != null)
                    _lbmContext.close();
            }
            catch
            {
            }
        }

        public void GetTradeAddImpact(TradeAddDetails tradeAddDetails, Action<TradeAddImpactResponseMessage> callback, Action<FailureCallbackInfo> callbackFailure)
        {
            TradeAddImpactRequestMessage requestMessage = new TradeAddImpactRequestMessage();
            requestMessage.TradeAddDetails = tradeAddDetails;

            CommandManager.AddCommand(
            new SendRequestDtoCommand
                <TradeAddImpactRequestMessage, TradeAddImpactRequestMessageDto,
                TradeAddImpactResponseMessage, TradeAddImpactResponseMessageDto>
                (InformaticaHelper.TradeAddImpactTopicName, requestMessage, callback, callbackFailure));
        }

        public void GetProductBreakdown(ProductBreakdownRequestMessage requestMessage, Action<ProductBreakdownResponseMessage> callback, Action<FailureCallbackInfo> callbackFailure)
        {
            CommandManager.AddCommand(
                new SendRequestCommand<ProductBreakdownRequestMessage, ProductBreakdownResponseMessage>(
                    InformaticaHelper.ProductBreakdownTopicName, requestMessage, callback, callbackFailure));
        }
    }
}