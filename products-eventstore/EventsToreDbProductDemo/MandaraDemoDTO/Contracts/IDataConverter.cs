namespace MandaraDemoDTO.Contracts
{
    public interface IDataConverter<T, U>
    {
        U? Convert(T value);
    }
}
