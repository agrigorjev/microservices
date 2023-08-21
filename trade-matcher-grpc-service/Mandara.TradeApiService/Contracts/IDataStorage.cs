
using Mandara.TradeApiService.Data;
using Optional;

namespace Mandara.TradeApiService.Contracts;

public interface IDataStorage
{

    void UpdatePortfolios();
    void UpdateOfficialProducts();

    IList<OfficialProduct> GetOfficialProducts();
    IList<Portfolio> GetPortfolios();

    Option<OfficialProduct> GetOfficialProduct(int id);
    Option<SecurityDefinition> GetSecurityDefinition(int id);
    Option<Portfolio> GetPortfolio(int portfolioId);
    Option<Product> GetProduct(int productId);

}