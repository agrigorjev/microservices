namespace MandaraDemoDTO
{
    public interface IDataConverter<T,U>
    {
        U? Convert(T value);
    }
}
