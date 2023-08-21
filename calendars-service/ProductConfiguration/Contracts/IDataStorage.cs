
using Mandara.ProductConfiguration.Data;
using Optional;

namespace Mandara.ProductConfiguration.Contracts;

public interface IDataStorage
{

    void Update();

    Option<OfficialProduct> GetMappings(int id);
    List<OfficialProduct> GetMappings();

    Option<ProductCategory> GetCategories(int id);
    List<ProductCategory> GetCategories();


}