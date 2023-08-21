using System.Collections.Concurrent;
using Optional;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using Mandara.ProductConfiguration.Contracts;
using Mandara.ProductConfiguration.Data;

namespace Mandara.ProductConfiguration.Services;


public class DataStorage : IDataStorage
{
    private ConcurrentDictionary<int,OfficialProduct> ProductMappings { get; set; } = new();
    private ConcurrentDictionary<int, ProductCategory> Categories    { get; set; } = new();


    private readonly IDbContextFactory<MandaraEntities> _contextFactory;

    public DataStorage(IDbContextFactory<MandaraEntities> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public void Update()
    {
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
            ProductMappings= new ConcurrentDictionary<int, OfficialProduct>(ProductMappingsQuery(productsDb).ToDictionary(x => x.OfficialProductId));
            Categories = new ConcurrentDictionary<int, ProductCategory>(CategoriesQuery(productsDb).ToDictionary(x => x.CategoryId));
        }
    }

    public Option<OfficialProduct> GetMappings(int id)
    {
        if (ProductMappings.TryGetValue(id,out OfficialProduct mapping))
        {
            return mapping.Some();
        }
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
           var optionOfMapping=ProductMappingsQuery(productsDb, id);
            if (optionOfMapping.HasValue)
            {
                var retrievedMapping = optionOfMapping.ValueOr(() => null);
                ProductMappings.TryAdd(retrievedMapping.OfficialProductId, retrievedMapping);
               
            }
            return optionOfMapping;
        }
    }

    public List<OfficialProduct> GetMappings()
    {
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
           return ProductMappings.Values.ToList();
           
        }
    }

    public Option<ProductCategory> GetCategories(int id)
    {
        if (Categories.TryGetValue(id, out ProductCategory mapping))
        {
            return mapping.Some();
        }
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
            var optionOfMapping = CategoriesQuery(productsDb, id);
            if (optionOfMapping.HasValue)
            {
                var retrievedMapping = optionOfMapping.ValueOr(() => null);
                Categories.TryAdd(retrievedMapping.CategoryId, retrievedMapping);

            }
            return optionOfMapping;
        }
    }

    public List<ProductCategory> GetCategories()
    {
       return Categories.Values.ToList();
    }


    private static List<OfficialProduct> ProductMappingsQuery(MandaraEntities cxt)
    {
        return cxt.OfficialProducts.Include(x => x.Products)
            .ToList()
            .Select(x =>
        {
            var pp = x.Products.OrderBy(p => p.ProductId).FirstOrDefault();
            if (pp != null)
            {
                x.Abbreviation = pp.Category?.Abbreviation;
                x.ConversionFactor = pp.PnlFactor;
                x.HolidayCalendarId = pp.HolidaysCalendarId;
                x.CategoryId = pp.CategoryId;
            }
            return x;
        }).ToList();
    }

    private static Option<OfficialProduct> ProductMappingsQuery(MandaraEntities cxt,int id)
    {
           var proposed=cxt.OfficialProducts
            .Where(op => op.OfficialProductId == id)
            .Include(x => x.Products)
            .FirstOrDefault();
        if (proposed != null)
        { 
                var pp = proposed.Products.OrderBy(p => p.ProductId).FirstOrDefault();
                if (pp != null)
                {
                    proposed.Abbreviation = pp.Category?.Abbreviation;
                    proposed.ConversionFactor = pp.PnlFactor;
                    proposed.HolidayCalendarId = pp.HolidaysCalendarId;
                    proposed.CategoryId = pp.CategoryId;
                }
            return proposed.Some();
        }
        return Option.None<OfficialProduct>();
    }

    private static List<ProductCategory>CategoriesQuery(MandaraEntities cxt)
    {
        return cxt.ProductCategories.Include(x => x.Products)
            .ToList()
            .Select(x =>
            {
                var pp = x.Products.OrderBy(p => p.ProductId).FirstOrDefault();
                if (pp != null)
                {
                    x.PnlFactor = pp.PnlFactor;
                }
                return x;
            }).ToList();
    }

    private static Option<ProductCategory> CategoriesQuery(MandaraEntities cxt, int id)
    {
        var proposed = cxt.ProductCategories.Include(x => x.Products)
         .Where(op => op.CategoryId == id)
         .Include(x => x.Products)
         .FirstOrDefault();
        if (proposed != null)
        {
            var pp = proposed.Products.OrderBy(p => p.ProductId).FirstOrDefault();
            if (pp != null)
            {
                proposed.PnlFactor = pp.PnlFactor;
            }
            return proposed.Some();
        }
        return Option.None<ProductCategory>();
    }
}

   