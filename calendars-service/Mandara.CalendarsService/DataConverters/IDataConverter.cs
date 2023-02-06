namespace Mandara.CalendarsService.DataConverters
{
    public interface IDataConverter<T, U>
    {
        U Convert(T data);

       
    }
}
