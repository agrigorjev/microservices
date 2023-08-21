namespace Mandara.TradeApiService.DataConverters
{
    public interface IDataConverter<T, U>
    {
        U Convert(T data);
    }
}
