namespace Mandara.ProductConfiguration.Contracts;

public interface IDataConverter<T, U>
{
    U Convert(T data);

   
}
