namespace Mandara.Business.Json
{
    public class ExecIdDatesKeyValuePairConverter : KeyValuePairConverterBase
    {
        public ExecIdDatesKeyValuePairConverter()
        {
            KeyPropertyName = "ExecId";
            ValuePropertyName = "TradeDates";
        }
    }
}