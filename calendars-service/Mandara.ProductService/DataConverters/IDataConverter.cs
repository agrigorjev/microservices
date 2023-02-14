namespace Mandara.ProductService.DataConverters
{
    public interface IDataConverter<T, U>
    {
        U? Convert(T data);

       
    }
}
