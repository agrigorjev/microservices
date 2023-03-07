namespace Mandara.Entities.Trades
{
    public class AssignTradeView : LiveTradeView
    {
        public bool DoAssign { get; set; }

        public string Comment { get; set; }

        public string SourceBook { get; set; }
    }
}