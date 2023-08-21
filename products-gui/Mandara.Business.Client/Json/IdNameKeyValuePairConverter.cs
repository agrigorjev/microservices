namespace Mandara.Business.Json
{
    public class IdNameKeyValuePairConverter : KeyValuePairConverterBase
    {
        public IdNameKeyValuePairConverter()
        {
            KeyPropertyName = "Id";
            ValuePropertyName = "Name";
        }
    }
}