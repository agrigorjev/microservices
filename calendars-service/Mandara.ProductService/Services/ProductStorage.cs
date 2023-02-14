using System.Collections.Concurrent;
using Optional;
using Microsoft.EntityFrameworkCore;
using Mandara.ProductService.Data;
using System.Collections.Generic;
using Mandara.ProductService.Data.Entities;
using Mandara.ProductService.Configuration;
using Microsoft.Extensions.Options;

namespace Mandara.ProductService.Services;

public class ProductStorage : IProductStorage
{



    private ConcurrentDictionary<int, Product> Products { get; set; } = new();
    private ConcurrentDictionary<int, SecurityDefinition> SecurityDefinitions { get; set; } = new();


    private readonly DataStoragesSettings _serviceSettings;
    private readonly IDbContextFactory<MandaraEntities> _contextFactory;

    public ProductStorage(IOptions<DataStoragesSettings> serviceSettings,
        IDbContextFactory<MandaraEntities> contextFactory)
    {
        _serviceSettings = serviceSettings.Value;
        _contextFactory = contextFactory;
    }



    public void Update()
    {
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
            Products = new ConcurrentDictionary<int, Product>(GetProductsQuery(productsDb).ToDictionary(p => p.ProductId));
            SecurityDefinitions = new ConcurrentDictionary<int, SecurityDefinition>(GetSecurityDefinitionQuery(productsDb).ToDictionary(p => p.SecurityDefinitionId));
        }
    }

    private Product LoadProduct(int id)
    {
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
            Product product = GetProductsQuery(productsDb).FirstOrDefault(p => p.ProductId == id) ?? Product.Default;
            if (!product.IsDefault)
            {
                Products.TryAdd(product.ProductId, product);
            }
            return product;
        }
    }



    private SecurityDefinition LoadDefinition(int id)
    {
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
            SecurityDefinition sd = productsDb.SecurityDefinitions.FirstOrDefault(s => s.SecurityDefinitionId == id) ?? new SecurityDefinition();
            if (!sd.IsNew())
            {
                SecurityDefinitions.TryAdd(sd.SecurityDefinitionId, sd);
            }
            return sd;
        }

    }



    private static IQueryable<Product> GetProductsQuery(MandaraEntities cxt)
    {
        return cxt.Products;
    }


    private static IQueryable<SecurityDefinition> GetSecurityDefinitionQuery(MandaraEntities cxt)
    {
        return cxt.SecurityDefinitions;
    }
    public List<Product> GetProducts()
    {
        return Products.Values.ToList();
    }

    public Option<Product> GetProduct(int id)
    {
        Product product = Product.Default;
        if(Products.ContainsKey(id) && Products.TryGetValue(id,out product))
        {
            return Option.Some(product);
        }
        else
        {
            product = LoadProduct(id);
        }
        return product.IsDefault?Option.None<Product>():Option.Some(product);

    }

    public List<SecurityDefinition> GetSecurityDefinitions()
    {
        return SecurityDefinitions.Values.ToList();
    }

    public Option<SecurityDefinition> GetSecurityDefinition(int id)
    {
        SecurityDefinition securityDefinition = new SecurityDefinition();
        if (SecurityDefinitions.ContainsKey(id) && SecurityDefinitions.TryGetValue(id, value: out securityDefinition))
        {
            return Option.Some(securityDefinition);
        }
        else
        {
            securityDefinition = LoadDefinition(id);
        }
        return securityDefinition.IsNew() ? Option.None<SecurityDefinition>() : Option.Some(securityDefinition);
    }
}

    


   