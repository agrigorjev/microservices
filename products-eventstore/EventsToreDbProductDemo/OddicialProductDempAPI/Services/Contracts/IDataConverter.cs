namespace OfficialProductDemoAPI.Services.Contracts
{
    public interface IDataConverter<T,U>
    {
        U? Convert(T value);
    }
}
