namespace Mandara.Business.Bus.Messages.EditTrades
{
    public class SecurityDefinitionDto2
    {
        public int SecurityDefinitionId { get; set; }
        public string UnderlyingUnitOfMeasure { get; set; }
        public string StripName { get; set; }
        public string ProductDescription { get; set; }
        public string Exchange { get; set; }
    }
}