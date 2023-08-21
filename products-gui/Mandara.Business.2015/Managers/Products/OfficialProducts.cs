using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Mandara.Business.Audit;
using Mandara.Business.Data;
using Mandara.Entities;
using Mandara.Entities.Constants;
using Mandara.Entities.Entities;
using Mandara.Entities.Enums;
using Mandara.Entities.Extensions;
using Ninject.Extensions.Logging;
using Optional;
using Optional.Collections;
using Optional.Unsafe;

namespace Mandara.Business.Managers.Products
{
    public class OfficialProductsManager
    {
        private readonly CurrencyProvider _currencies = new CurrencyProvider();

        private ConcurrentDictionary<int, OfficialProduct> _officialProducts =
            new ConcurrentDictionary<int, OfficialProduct>();
        private readonly UnitsProvider _units = new UnitsProvider();

        private static readonly ILogger Logger = new NLogLoggerFactory().GetCurrentClassLogger();

        internal event EventHandler<OfficialProductChangeEventArgs> OfficialProductAdded;
        internal event EventHandler<OfficialProductChangeEventArgs> OfficialProductDeleted;
        internal event EventHandler<OfficialProductChangeEventArgs> OfficialProductChanged;

        private static MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(ProductManager));
        }

        public OfficialProduct GetOfficialProduct(string displayName)
        {
            OfficialProduct existingOffProduct = _officialProducts
                                                 .FirstOrNone(offProd => displayName == offProd.Value.DisplayName)
                                                 .ValueOr(
                                                     new KeyValuePair<int, OfficialProduct>(
                                                         OfficialProduct.DefaultId,
                                                         OfficialProduct.Default)).Value;

            if (existingOffProduct.IsDefault())
            {
                using (MandaraEntities context = CreateMandaraProductsDbContext())
                {
                    existingOffProduct = GetAllOfficialProductsFromDb(context).SingleOrDefault(
                                                    offProd => offProd.DisplayName.Equals(
                                                        displayName,
                                                        StringComparison
                                                            .InvariantCultureIgnoreCase)) ?? OfficialProduct.Default;

                    if (!existingOffProduct.IsDefault())
                    {
                        _officialProducts.TryAdd(existingOffProduct.OfficialProductId, existingOffProduct);
                    }
                }
            }

            return existingOffProduct;
        }

        private static IQueryable<OfficialProduct> GetAllOfficialProductsFromDb(MandaraEntities context)
        {
            return context.OfficialProducts.Include(offProd => offProd.ParserWords)
                          // According to the types, this field doesn't exist.
                          .Include("ParserWords.Broker").Include(offProd => offProd.Region)
                          .Include(offProd => offProd.Currency)
                          .Include(offProd => offProd.PriceUnit)
                          .Include(offProd => offProd.PlattsSymbolConfig);
        }

        public OfficialProduct GetOfficialProduct(int productId)
        {
            if (!_officialProducts.TryGetValue(productId, out OfficialProduct offProduct))
            {
                using (MandaraEntities context = CreateMandaraProductsDbContext())
                {
                    offProduct = GetAllOfficialProductsFromDb(context)
                                     .SingleOrDefault(offProd => offProd.OfficialProductId == productId)
                                 ?? OfficialProduct.Default;

                    if (!offProduct.IsDefault())
                    {
                        _officialProducts.TryAdd(productId, offProduct);
                    }
                }
            }

            return offProduct;
        }

        public Option<OfficialProduct> GetDefaultOfficialProduct()
        {
            (Option<Currency> defaultCurr, Option<Unit> productUnit) = GetDefaultCurrencyAndUnit();

            if (!defaultCurr.HasValue)
            {
                return Option.None<OfficialProduct>();
            }

            return Option.Some(
                new OfficialProduct
                {
                    Name = "New official product",
                    Currency = defaultCurr.ValueOrFailure(),
                    PriceUnit = productUnit.ValueOrFailure(),
                    UnitToBarrelConversionFactor = 1m
                });
        }

        private Tuple<Option<Currency>, Option<Unit>> GetDefaultCurrencyAndUnit()
        {
            Option<Currency> usd = _currencies.GetCurrency(CurrencyCodes.USD);
            Option<Currency> defaultCurr = usd.HasValue ? usd : _currencies.GetFirstAvailableCurrency();
            Option<Unit> bbl = _units.GetUnit(UnitSymbols.Barrel);
            Option<Unit> productUnit = bbl.HasValue ? bbl : _units.GetFirstAvailableUnit();

            return new Tuple<Option<Currency>, Option<Unit>>(defaultCurr, productUnit);
        }

        public List<OfficialProduct> GetOfficialProducts()
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                List<OfficialProduct> allOfficialProducts = GetAllOfficialProductsFromDb(context).ToList();

                allOfficialProducts.ForEach(
                    offProd => _officialProducts.AddOrUpdate(
                        offProd.OfficialProductId,
                        offProd,
                        (id, existingOffProd) => offProd));
                return allOfficialProducts;
            }
        }

        public (int, bool) SaveOfficialProduct(OfficialProduct offProdToSave, AuditContext auditContext)
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Func<int> newIdFunc = SaveOfficialProduct(offProdToSave, context, transaction);
                    int idBeforeSave = offProdToSave.OfficialProductId;
                    int idAfterSave = newIdFunc();

                    offProdToSave.OfficialProductId = idAfterSave;
                    PostProcessSave(offProdToSave, idBeforeSave, idAfterSave);
                    return (offProdToSave.OfficialProductId, true);

                }
                catch (SqlException saveFailure)
                {
                    LogDbActionError(offProdToSave, saveFailure, "save");
                    return (offProdToSave.OfficialProductId, false);
                }
                catch (DbUpdateException saveFailure)
                {
                    LogDbActionError(offProdToSave, saveFailure, "save");
                    return (offProdToSave.OfficialProductId, false);
                }
            }
        }

        private void PostProcessSave(OfficialProduct offProdToSave, int idBeforeSave, int idAfterSave)
        {
            if (idBeforeSave == OfficialProduct.DefaultId)
            {
                OnAddOfficialProduct();
            }
            else if (_officialProducts.TryGetValue(idBeforeSave, out OfficialProduct existingOffProd))
            {
                OnModifyKnownOfficialProduct(existingOffProd);
            }
            else
            {
                OnModifyUnseenExistingOfficialProduct();
            }

            void OnAddOfficialProduct()
            {
                GetOfficialProduct(idAfterSave);
                OfficialProductAdded?.Invoke(
                    this,
                    new OfficialProductChangeEventArgs(offProdToSave, OfficialProductChangeType.Added));
            }

            void OnModifyKnownOfficialProduct(OfficialProduct existingOffProd)
            {
                if (offProdToSave.Name != existingOffProd.Name)
                {
                    _officialProducts.AddOrUpdate(idBeforeSave, offProdToSave, (id, existing) => offProdToSave);
                    OfficialProductChanged?.Invoke(
                        this,
                        new OfficialProductChangeEventArgs(
                            offProdToSave,
                            existingOffProd.Name,
                            OfficialProductChangeType.NameChange));
                }

                existingOffProd.PlattsSymbolConfig = offProdToSave.PlattsSymbolConfig;
            }

            void OnModifyUnseenExistingOfficialProduct()
            {
                OfficialProduct unknownExistingProduct = OfficialProduct.Default;

                unknownExistingProduct.OfficialProductId = idBeforeSave;
                OnModifyKnownOfficialProduct(unknownExistingProduct);
            }
        }

        private static Func<int> SaveOfficialProduct(
            OfficialProduct offProdToSave,
            MandaraEntities context,
            DbContextTransaction transaction)
        {
            context.Configuration.AutoDetectChangesEnabled = false;
            context.Save(
                context.OfficialProducts,
                offProdToSave,
                offProd => offProd.OfficialProductId,
                out Func<int> newIdFunc).Save(
                context.ParserWords,
                offProdToSave.ParserWords,
                offProd => offProd.WordId,
                offProd => offProd.OfficialProductId == offProdToSave.OfficialProductId);
            
            PlattsSymbolConfig previousSymbolConfig = context.PlattsSymbolConfig.Find(offProdToSave.OfficialProductId);
            if (offProdToSave.PlattsSymbolConfig == null)
            {
                if (previousSymbolConfig != null)
                {
                    context.PlattsSymbolConfig.Remove(previousSymbolConfig);
                }
            }
            else
            {
                if (previousSymbolConfig == null)
                {
                    context.PlattsSymbolConfig.Add(offProdToSave.PlattsSymbolConfig);
                }
                else
                {
                    previousSymbolConfig.Update(offProdToSave.PlattsSymbolConfig);
                }
            }
            context.ChangeTracker.DetectChanges();
            context.SaveChanges();
            transaction.Commit();
            return newIdFunc;
        }

        public bool DeleteOfficialProduct(OfficialProduct offProdToDelete, AuditContext unusedAuditContext)
        {
            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            {
                if (OfficialProductIsReferencedByProduct(offProdToDelete.OfficialProductId, dbContext))
                {
                    return false;
                }

                try
                {
                    DeleteFromDatabase(offProdToDelete, dbContext);
                    _officialProducts.TryRemove(offProdToDelete.OfficialProductId, out _);
                    OfficialProductDeleted?.Invoke(
                        this,
                        new OfficialProductChangeEventArgs(offProdToDelete, OfficialProductChangeType.Removed));
                    return true;
                }
                catch (SqlException deleteError)
                {
                    LogDbActionError(offProdToDelete, deleteError, "delete");
                    return false;
                }
                catch (DbUpdateException deleteError)
                {
                    LogDbActionError(offProdToDelete, deleteError, "delete");
                    return false;
                }
            }
        }

        private static void LogDbActionError(OfficialProduct offProdToSave, Exception dbActionError, string action)
        {
            Logger.Error(
                dbActionError,
                "Error trying to {0} official product '{1}' [{2}]",
                action,
                offProdToSave.Name,
                offProdToSave.OfficialProductId);
        }

        private static void DeleteFromDatabase(OfficialProduct offProdToDelete, MandaraEntities dbContext)
        {
            OfficialProduct existingProduct = dbContext.OfficialProducts.Include(offProd => offProd.ParserWords)
                                                       .Single(
                                                           p => p.OfficialProductId
                                                                == offProdToDelete.OfficialProductId);

            dbContext.OfficialProducts.Remove(existingProduct);
            dbContext.SaveChanges();
        }

        private static bool OfficialProductIsReferencedByProduct(
            int offProdId,
            MandaraEntities dbContext)
        {
            return IsOfficialProductReferencedInExistingProducts()
                   || IsOfficialProductReferencedInExistingDefaultProducts();

            bool IsOfficialProductReferencedInExistingProducts()
            {
                return dbContext.Products.Include(product => product.OfficialProduct).Any(
                    product => product.OfficialProduct.OfficialProductId == offProdId);
            }

            bool IsOfficialProductReferencedInExistingDefaultProducts()
            {
                return dbContext.ParserDefaultProducts.Include(product => product.OfficialProduct).Any(
                    product => product.OfficialProduct.OfficialProductId == offProdId);
            }
        }
    }
}
