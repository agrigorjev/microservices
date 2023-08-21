namespace Mandara.TradeApiService.Data;

public class TradeCaptureUniqueKey
{
    public DateTime? TradeDate { get; set; }
    public string ClOrdID { get; set; }
    public string ExecID { get; set; }
    public string LegRefID { get; set; }
    public string Exchange { get; set; }

    public override string ToString()
    {
        return $"{Exchange} - {TradeDate} - {ClOrdID} - {ExecID} - {LegRefID}";
    }
}
