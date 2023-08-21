namespace Mandara.Business.Json
{
    public class SourceDataKeyValuePairConverter : KeyValuePairConverterBase
    {
        public SourceDataKeyValuePairConverter()
        {
            KeyPropertyName = "Date";
            ValuePropertyName = "DataType";
        }
    }
}