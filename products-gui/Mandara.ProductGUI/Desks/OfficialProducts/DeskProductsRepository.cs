using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Mandara.Database.Query;
using Mandara.Entities;
using Mandara.Extensions.Collections;
using Ninject.Extensions.Logging;
using Optional.Collections;

namespace Mandara.ProductGUI.Desks.OfficialProducts
{
    public class DeskProductsRepository : IDeskProductsRepository
    {
        private static readonly ILogger Logger = new NLogLoggerFactory().GetCurrentClassLogger();
        private readonly Dictionary<int, List<OfficialProductForDesk>> _officialProductsByDesk;
        private readonly Dictionary<int, HashSet<int>> _officialProductIdsByDesk;
        private const int OfficialProdIdColumn = 0;
        private const int DeskIdColumn = 1;
        private const int ProductTypeColumn = 2;
        private const int PricePositionColumn = 3;
        private const int PriceFactorColumn = 4;
        private const int CacheLifeTimeMinutesColumn = 5;
        private const int ShowPriceAsAgedColumn = 6;
        private const int AlertGroupIdColumn = 7;
        private const int OrderColumn = 8;

        private const string OfficialProductFields =
            @"OfficialProductId, 
                DeskId, 
                ProductType, 
                PricePosition, 
                PriceFactor,
                CacheLifetimeMinutes, 
                ShowPriceAsAged, 
                AlertGroupId, 
                [Order]";

        private static readonly string SelectOfficialProducts =
            $"SELECT {OfficialProductFields} FROM dbo.OfficialProductSettings";
        private static readonly string InsertOfficialProductTemplate =
            $@"INSERT INTO OfficialProductSettings ({OfficialProductFields}) 
                VALUES ({{0}}, {{1}}, {{2}}, {{3}}, {{4}}, {{5}}, {{6}}, {{7}}, {{8}})";
        private static readonly string UpdateOfficialProductTemplate =
            @"UPDATE OfficialProductSettings 
                SET OfficialProductId = {0},
                DeskId = {1},
                ProductType = {2},
                PricePosition = {3},
                PriceFactor = {4},
                CacheLifetimeMinutes = {5},
                ShowPriceAsAged = {6},
                AlertGroupId = {7},
                [Order] = {8} 
              WHERE OfficialProductId={9} AND DeskId={10} AND ProductType={11}";
        private const string DeleteOfficialProductForTypeTemplate =
            "DELETE FROM OfficialProductSettings WHERE OfficialProductId={0} AND DeskId={1} AND ProductType={2}";
        private const string DeleteOfficialProductTemplate =
            "DELETE FROM OfficialProductSettings WHERE OfficialProductId={0}";

        public DeskProductsRepository()
        {
            _officialProductsByDesk = new Dictionary<int, List<OfficialProductForDesk>>();
            _officialProductIdsByDesk = new Dictionary<int, HashSet<int>>();
        }

        public void LoadProducts()
        {
            List<OfficialProductForDesk> allOfficialProducts = SqlServerCommandExecution.ReadToList(
                DesksConfiguration.ConnectionStringName,
                SelectOfficialProducts,
                CreateOfficialProductForDesk);

            AddProductsByDesk(allOfficialProducts);

            OfficialProductForDesk CreateOfficialProductForDesk(SqlDataReader reader)
            {
                return new OfficialProductForDesk(
                    reader.GetInt32(OfficialProdIdColumn),
                    reader.GetInt32(DeskIdColumn),
                    (StripStructure)reader.GetInt32(ProductTypeColumn),
                    reader.GetInt32(PricePositionColumn),
                    (decimal)reader.GetDouble(PriceFactorColumn),
                    reader.GetInt32(CacheLifeTimeMinutesColumn),
                    reader.GetInt32(ShowPriceAsAgedColumn),
                    GetAlertGroupId(reader),
                    reader.GetInt32(OrderColumn));
            }

            int GetAlertGroupId(SqlDataReader reader)
            {
                return !reader.IsDBNull(AlertGroupIdColumn)
                    ? reader.GetInt32(AlertGroupIdColumn)
                    : OfficialProductForDesk.NoAlertGroup;
            }
        }

        private void AddProductsByDesk(List<OfficialProductForDesk> allOfficialProducts)
        {
            allOfficialProducts.GroupBy(offProd => offProd.DeskId).ForEach(
                deskIdAndProducts =>
                {
                    _officialProductsByDesk[deskIdAndProducts.Key] = deskIdAndProducts.ToList();
                    _officialProductIdsByDesk[deskIdAndProducts.Key] = new HashSet<int>(
                        deskIdAndProducts.Select(offProd => offProd.OfficialProductId));
                });
        }

        public List<Product> ProductsForDesk(List<Product> allValidProducts, int deskId)
        {
            List<int> deskOfficialProducts = GetDeskProductIds(deskId);

            return allValidProducts.Where(product => deskOfficialProducts.Contains(product.OfficialProductId)).ToList();
        }

        private List<int> GetDeskProductIds(int deskId)
        {
            return _officialProductIdsByDesk.TryGetValue(deskId, out HashSet<int> offProdIds)
                ? offProdIds.ToList()
                : new List<int>();
        }

        public List<OfficialProductForDesk> OfficialProductsForDesk(int deskId)
        {
            return _officialProductsByDesk.TryGetValue(deskId, out List<OfficialProductForDesk> products)
                ? products
                : 0 == deskId ? new List<OfficialProductForDesk>() : AddEmptyProductsForDesk(deskId);
        }

        private List<OfficialProductForDesk> AddEmptyProductsForDesk(int deskId)
        {
            _officialProductsByDesk[deskId] = new List<OfficialProductForDesk>();
            _officialProductIdsByDesk[deskId] = new HashSet<int>();

            return _officialProductsByDesk[deskId];
        }

        public bool ProductExists(OfficialProductForDesk testProduct)
        {
            return !TryGetExistingProduct(testProduct).IsDefault();
        }

        private OfficialProductForDesk TryGetExistingProduct(OfficialProductForDesk testProduct)
        {
            return OfficialProductsForDesk(testProduct.DeskId).FirstOrNone(
                offProd =>
                    testProduct.OfficialProductId == offProd.Key.OfficialProductId
                    && testProduct.ProductType == offProd.Key.ProductType).ValueOr(
                OfficialProductForDesk.Default);
        }

        public DatabaseActionResult Add(OfficialProductForDesk newProduct)
        {
            try
            {
                if (ProductExists(newProduct))
                {
                    string nonExistentProduct = string.Format(
                        "Cannot add existing official product {0} on desk {1} with type {2}.",
                        newProduct.OfficialProductId,
                        newProduct.DeskId,
                        newProduct.ProductType);

                    Logger.Info(nonExistentProduct);
                    return new DatabaseActionResult(false, nonExistentProduct);
                }

                SqlServerCommandExecution.ExecuteNonQuery(
                    DesksConfiguration.ConnectionStringName,
                    string.Format(
                        InsertOfficialProductTemplate,
                        newProduct.OfficialProductId,
                        newProduct.DeskId,
                        (int)newProduct.ProductType,
                        newProduct.PricePosition,
                        newProduct.PriceFactor,
                        newProduct.CacheLifeTimeMinutes,
                        newProduct.ShowPriceAsAged,
                        GetAlertGroupIdForDb(newProduct),
                        newProduct.Order));

                AddDeskProduct(newProduct.UpdateKey());
                return new DatabaseActionResult(true, string.Empty);
            }
            catch (SqlException insertError)
            {
                LogDatabaseAccessError("insert", newProduct, insertError);
                return new DatabaseActionResult(false, insertError.Message);
            }
        }

        private static string GetAlertGroupIdForDb(OfficialProductForDesk product)
        {
            return product.AlertGroupId != OfficialProductForDesk.NoAlertGroup
                ? product.AlertGroupId.ToString()
                : "NULL";
        }

        private void AddDeskProduct(OfficialProductForDesk newProduct)
        {
            AddDeskProductId(newProduct.DeskId, newProduct.OfficialProductId);
        }

        private void AddDeskProductId(int deskId, int officialProductId)
        {
            if (!_officialProductIdsByDesk.TryGetValue(deskId, out HashSet<int> deskProducts))
            {
                deskProducts = new HashSet<int>();
                _officialProductIdsByDesk[deskId] = deskProducts;
            }

            deskProducts.Add(officialProductId);
        }

        private void LogDatabaseAccessError(
            string dbAction,
            OfficialProductForDesk productGeneratingError,
            SqlException sqlError)
        {
            Logger.Error(
                sqlError,
                "Failed to {0} official product settings for product {1} on desk {2} with type {3}",
                dbAction,
                productGeneratingError.OfficialProductId,
                productGeneratingError.DeskId,
                productGeneratingError.ProductType);
        }

        public DatabaseActionResult Update(OfficialProductForDesk changedProduct)
        {
            try
            {
                OfficialProductForDesk previousSettings = TryGetPreviousProduct(changedProduct);

                if (previousSettings.IsDefault())
                {
                    Logger.Info(
                        "Cannot update non-existent official product settings for product {0} on desk {1} with type "
                            + "{2}.  Inserting instead.",
                        changedProduct.OfficialProductId,
                        changedProduct.DeskId,
                        changedProduct.ProductType);
                    return Add(changedProduct);
                }

                SqlServerCommandExecution.ExecuteNonQuery(
                    DesksConfiguration.ConnectionStringName,
                    string.Format(
                        UpdateOfficialProductTemplate,
                        changedProduct.OfficialProductId,
                        changedProduct.DeskId,
                        (int)changedProduct.ProductType,
                        changedProduct.PricePosition,
                        changedProduct.PriceFactor,
                        changedProduct.CacheLifeTimeMinutes,
                        changedProduct.ShowPriceAsAged,
                        GetAlertGroupIdForDb(changedProduct),
                        changedProduct.Order,
                        previousSettings.Key.OfficialProductId,
                        previousSettings.Key.DeskId,
                        (int)previousSettings.Key.ProductType));

                UpdateOfficialProductForDesk(changedProduct, previousSettings);
                return new DatabaseActionResult(true, string.Empty);
            }
            catch (SqlException updateError)
            {
                LogDatabaseAccessError("update", changedProduct, updateError);
                return new DatabaseActionResult(false, updateError.Message);
            }
        }

        private OfficialProductForDesk TryGetPreviousProduct(OfficialProductForDesk testProduct)
        {
            return OfficialProductsForDesk(testProduct.DeskId).FirstOrNone(
                                        offProd => testProduct.Key.OfficialProductId == offProd.Key.OfficialProductId
                                                   && testProduct.Key.ProductType == offProd.Key.ProductType)
                                    .ValueOr(OfficialProductForDesk.Default);
        }

        private void UpdateOfficialProductForDesk(
            OfficialProductForDesk changedProduct,
            OfficialProductForDesk previousSettings)
        {
            UpdateInProductsByDesk(changedProduct);
            UpdateInProductIdsByDesk(
                changedProduct.DeskId,
                changedProduct.OfficialProductId,
                previousSettings.OfficialProductId);
        }

        private void UpdateInProductsByDesk(OfficialProductForDesk changedProduct)
        {
            // Because the products by desk are manipulated directly by the UI the only change required here is to
            // ensure the product Key is up to date.
            changedProduct.UpdateKey();
        }

        private void UpdateInProductIdsByDesk(int deskId, int changedOffProdId, int previousOffProdId)
        {
            _officialProductIdsByDesk[deskId].Remove(previousOffProdId);
            _officialProductIdsByDesk[deskId].Add(changedOffProdId);
        }

        public DatabaseActionResult Delete(OfficialProductForDesk productToDelete)
        {
            try
            {
                if (!ProductExists(productToDelete))
                {
                    string nonExistentProduct = string.Format(
                        "Cannot remove non-existent official product settings for product {0} on desk {1} with type "
                        + "{2}.",
                        productToDelete.OfficialProductId,
                        productToDelete.DeskId,
                        productToDelete.ProductType);

                    Logger.Error(nonExistentProduct);
                    return new DatabaseActionResult(false, nonExistentProduct);
                }

                SqlServerCommandExecution.ExecuteNonQuery(
                    DesksConfiguration.ConnectionStringName,
                    string.Format(
                        DeleteOfficialProductForTypeTemplate,
                        productToDelete.OfficialProductId,
                        productToDelete.DeskId,
                        (int)productToDelete.ProductType));

                RemoveOfficialProductForDesk(productToDelete);

                return new DatabaseActionResult(true, string.Empty);
            }
            catch (SqlException deleteError)
            {
                LogDatabaseAccessError("delete", productToDelete, deleteError);
                return new DatabaseActionResult(false, deleteError.Message);
            }
        }

        private void RemoveOfficialProductForDesk(OfficialProductForDesk productToDelete)
        {
            RemoveFromProductByDesk(productToDelete);
            RemoveFromProductIdsByDesk(productToDelete);
        }

        private void RemoveFromProductByDesk(OfficialProductForDesk productToDelete)
        {
            OfficialProductsForDesk(productToDelete.DeskId).RemoveAll(
                offProd => offProd.OfficialProductId == productToDelete.OfficialProductId
                        && offProd.ProductType == productToDelete.ProductType);
        }

        private void RemoveFromProductIdsByDesk(OfficialProductForDesk productToDelete)
        {
            if (ProductExists(productToDelete.OfficialProductId, productToDelete.DeskId))
            {
                return;
            }

            _officialProductIdsByDesk[productToDelete.DeskId]
                .Remove(offProdId => offProdId == productToDelete.OfficialProductId);
        }

        private bool ProductExists(int officialProduct, int desk)
        {
            return _officialProductsByDesk[desk].Any(offProd => officialProduct == offProd.OfficialProductId);
        }

        public DatabaseActionResult Delete(int officialProduct)
        {
            try
            {
                if (!ProductExists(officialProduct))
                {
                    string nonExistentProduct = String.Format(
                        "Cannot remove non-existent official product settings for product {0}.",
                        officialProduct);

                    Logger.Error(nonExistentProduct);
                    return new DatabaseActionResult(false, nonExistentProduct);
                }

                SqlServerCommandExecution.ExecuteNonQuery(
                    DesksConfiguration.ConnectionStringName,
                    string.Format(DeleteOfficialProductTemplate, officialProduct));

                RemoveOfficialProductForDesk(officialProduct);
                return new DatabaseActionResult(true, string.Empty);
            }
            catch (SqlException deleteError)
            {
                LogDatabaseAccessError("delete", officialProduct, deleteError);
                return new DatabaseActionResult(false, deleteError.Message);
            }
        }

        private bool ProductExists(int officialProduct)
        {
            return _officialProductsByDesk.Any(
                offProdsForDesk =>
                    offProdsForDesk.Value.Count(offProd => officialProduct == offProd.Key.OfficialProductId) > 0);
        }

        private void RemoveOfficialProductForDesk(int officialProduct)
        {
            RemoveAllFromProductByDesk(officialProduct);
            RemoveAllFromProductIdsByDesk(officialProduct);
        }

        private void RemoveAllFromProductByDesk(int officialProduct)
        {
            _officialProductsByDesk.ForEach(offProdsForDesk =>
                offProdsForDesk.Value.RemoveAll(offProd => offProd.OfficialProductId == officialProduct));
        }

        private void RemoveAllFromProductIdsByDesk(int officialProduct)
        {
            _officialProductIdsByDesk.ForEach(offProdsForDesk =>
                offProdsForDesk.Value.Remove(offProdId => offProdId == officialProduct));
        }

        private void LogDatabaseAccessError(
            string dbAction,
            int officialProduct,
            SqlException sqlError)
        {
            Logger.Error(
                sqlError,
                "Failed to {0} official product settings for product {1}",
                dbAction,
                officialProduct);
        }

        public int TotalProducts()
        {
            return _officialProductsByDesk.Sum(offProds => offProds.Value.Count);
        }
    }
}