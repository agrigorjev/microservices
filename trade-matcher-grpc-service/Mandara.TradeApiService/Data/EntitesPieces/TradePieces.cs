namespace Mandara.TradeApiService.Data;

public sealed class TradePieces
{
    public TradeCapture Trade { get; }
    public SecurityDefPieces Security { get; }

    public TradePieces(TradeCapture trade) : this(trade, trade.SecurityDefinition, trade.SecurityDefinition.Product)
    {
    }

    public TradePieces(TradeCapture trade, SecurityDefinition secDef) : this(trade, secDef, secDef.Product)
    {
    }

    public TradePieces(TradeCapture trade, SecurityDefinition secDef, Product product)
    {
        Trade = trade;
        Security = new SecurityDefPieces(secDef, product);
    }

    public TradePieces(TradeCapture trade, SecurityDefPieces secDef) : this(
        trade,
        secDef.SecurityDef,
        secDef.Product)
    {
    }

    public Strip Strip => Trade.Strip.IsDefault() ? StripParser.Parse(this) : Trade.Strip;

    public TradeCaptureUniqueKey UniqueKey => Trade.UniqueKey;

    public override string ToString()
    {
        return $"Trade - [{Trade}]; {Security}";
    }
}
