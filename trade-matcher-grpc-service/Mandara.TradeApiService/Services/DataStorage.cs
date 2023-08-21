using System.Collections.Concurrent;
using Optional;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using Mandara.TradeApiService.Contracts;
using Mandara.TradeApiService.Data;
using Mandara.TradeApiService.Configuration;
using Mandara.TradeApiService.Data;

namespace Mandara.TradeApiService.Services;


public class DataStorage : IDataStorage
{
    private readonly DataStoragesSettings _serviceSettings;
    private readonly IDbContextFactory<MandaraEntities> _contextFactory;
    private ConcurrentDictionary<int, Portfolio> Portfolios { get; set; } = new();
    private ConcurrentDictionary<int, Unit> Units { get; set; } = new();
    private ConcurrentDictionary<int, TradeTemplate> TradeTemplates { get; set; } = new();
    private ConcurrentDictionary<int, Company> Brokers { get; set; } = new();
    private ConcurrentDictionary<int, Exchange> Exchanges { get; set; } = new();
    private ConcurrentDictionary<int, OfficialProduct> OfficialProducts { get; set; } = new();
    private ConcurrentDictionary<int, SecurityDefinition> SecurityDefinitions { get; set; } = new();
    private ConcurrentDictionary<int, Product> Products { get; set; } = new();

    private List<Portfolio> _portfolios = new();
    private List<Unit> _units = new();
    private List<TradeTemplate> _tradeTemplates = new();
    private List<Company> _brokers = new();
    private List<Exchange> _exchanges = new();
    private List<OfficialProduct> _officialProducts = new();


    public DataStorage(IOptions<DataStoragesSettings> serviceSettings, IDbContextFactory<MandaraEntities> contextFactory)
    {
        _serviceSettings = serviceSettings.Value;
        _contextFactory = contextFactory;
    }

    public void UpdatePortfolios()
    {
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
            _portfolios = GetPortfolioQuery(productsDb).ToList();
        }

        Portfolio? errorBook = _portfolios.FirstOrDefault(portfolio => portfolio.PortfolioId == _serviceSettings.ErrorBookId);

        if (errorBook != null)
        {
            errorBook.IsErrorBook = true;
        }

        Portfolios =
            new ConcurrentDictionary<int, Portfolio>(_portfolios.ToDictionary(portfolio => portfolio.PortfolioId));

    }

    public void UpdateOfficialProducts()
    {
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
            OfficialProducts = new ConcurrentDictionary<int, OfficialProduct>(GetOfficialProductsQuery(productsDb).ToDictionary(p => p.OfficialProductId));
        }
    }
    private static IQueryable<OfficialProduct> GetOfficialProductsQuery(MandaraEntities cxt)
    {
        return cxt.OfficialProducts
            .Include(x => x.Products)
            .ThenInclude(x => x.Unit)
            .Include(x => x.Currency);
    }

    private static IQueryable<SecurityDefinition> GetSecurityDefitionsQuery(MandaraEntities cxt)
    {
        return cxt.SecurityDefinitions;
    }

    private static IQueryable<Product> GetProductsQuery(MandaraEntities cxt)
    {
        return cxt.Products;
    }

    private static IQueryable<Portfolio> GetPortfolioQuery(MandaraEntities cxt)
    {
        return cxt.Portfolios.Include(x => x.ParentPortfolio).Include(x => x.Portfolios);
    }

    public Option<Portfolio> GetPortfolio(int portfolioId)
    {
        throw new NotImplementedException();
    }

    public Option<SecurityDefinition> GetSecurityDefinition(int securityDefinitionId)
    {
        SecurityDefinition? secdef = null;
        if (SecurityDefinitions.ContainsKey(securityDefinitionId) && SecurityDefinitions.TryGetValue(securityDefinitionId, out secdef))
        {
            return Option.Some(secdef);
        }
        else
        {
            secdef = LoadSecurityDefinition(securityDefinitionId);
        }
        return secdef == null ? Option.None<SecurityDefinition>() : Option.Some(secdef);
    }

    public Option<Product> GetProduct(int productId)
    {
        Product? product = null;
        if (Products.ContainsKey(productId) && Products.TryGetValue(productId, out product))
        {
            return Option.Some(product);
        }
        else
        {
            product = LoadProduct(productId);
        }
        return product == null ? Option.None<Product>() : Option.Some(product);
    }

    private Product LoadProduct(int id)
    {
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
            Product? product = GetProductsQuery(productsDb).FirstOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                Products.TryAdd(product.ProductId, product);
            }
            return product;
        }
    }

    private SecurityDefinition LoadSecurityDefinition(int id)
    {
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
            SecurityDefinition? secdef = GetSecurityDefitionsQuery(productsDb).FirstOrDefault(p => p.SecurityDefinitionId == id);
            if (secdef != null)
            {
                SecurityDefinitions.TryAdd(secdef.SecurityDefinitionId, secdef);
            }
            return secdef;
        }
    }

    public IList<OfficialProduct> GetOfficialProducts()
    {
        return OfficialProducts.Values.ToList();
    }

    public Option<OfficialProduct> GetOfficialProduct(int id)
    {
        OfficialProduct? offproduct = OfficialProduct.Default;
        if (OfficialProducts.ContainsKey(id) && OfficialProducts.TryGetValue(id, out offproduct))
        {
            return Option.Some(offproduct);
        }
        else
        {
            offproduct = LoadOfficialProduct(id);
        }
        return offproduct.IsDefault ? Option.None<OfficialProduct>() : Option.Some(offproduct);
    }

    private OfficialProduct LoadOfficialProduct(int id)
    {
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
            OfficialProduct product = GetOfficialProductsQuery(productsDb).FirstOrDefault(p => p.OfficialProductId == id) ?? OfficialProduct.Default;
            if (!product.IsDefault)
            {
                OfficialProducts.TryAdd(product.OfficialProductId, product);
            }
            return product;
        }
    }


    public IList<Portfolio> GetPortfolios()
    {
        return _portfolios;
    }
}

