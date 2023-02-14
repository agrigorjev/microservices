using Mandara.ProductService.Data.Entities;
using Optional;
using System.Runtime.InteropServices;

namespace Mandara.ProductService.Services;

public interface IProductStorage
{

    void Update();

    List<Product> GetProducts();

    Option<Product> GetProduct(int id);

    List<SecurityDefinition> GetSecurityDefinitions();

    Option<SecurityDefinition> GetSecurityDefinition(int id);

}