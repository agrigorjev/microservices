namespace Mandara.TradeApiService.Contracts;

public interface IDataConverter<T, U>
{
    U Convert(T data);

   
}
