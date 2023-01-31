namespace Mandara.DataService.DataConverters
{
    public interface IDataConverter<T, U>
    {
        U Convert(T data);
    }
}
