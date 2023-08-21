namespace Mandara.Entities.EntityPieces
{
    public sealed class FxTradePieces
    {
        public FxTrade FX { get; }

        public TradePieces Trade { get; }

        public FxTradePieces(FxTrade trade) : this(trade, new TradePieces(trade.TradeCapture))
        {
        }

        public FxTradePieces(FxTrade fx, TradePieces trade)
        {
            NullTester.ThrowIfNullArgument(fx, nameof(trade), "FxTradePieces");

            FX = fx;
            Trade = trade;
        }
    }
}
