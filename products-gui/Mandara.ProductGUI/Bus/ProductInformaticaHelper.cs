using Mandara.Business.Bus;
using Mandara.Business.Bus.Handlers.Base;
using com.latencybusters.lbm;

namespace Mandara.ProductGUI.Bus
{
    public class ProductInformaticaHelper : InformaticaHelper
    {
        public ProductInformaticaHelper(LBMContext lbmContext, HandlerManager handlerManager) : base(lbmContext, handlerManager)
        {
        }

        public override void CreateSources()
        {
            AddSource(ResetServerTopicName);
            AddSource(TradeAddImpactTopicName);
            AddSource(FeaturesTopicName);
            AddSource(ProductBreakdownTopicName);

            base.CreateSources();
        }
    }
}