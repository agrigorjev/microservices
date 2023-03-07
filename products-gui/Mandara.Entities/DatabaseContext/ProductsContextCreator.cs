using System;

namespace Mandara.Entities.DatabaseContext
{
    public class ProductsContextCreator
    {
        private readonly string _connectionString;

        public ProductsContextCreator(string connectionString)
        {
            _connectionString = connectionString
                                ?? throw new ArgumentNullException(
                                    $"{nameof(ProductsContextCreator)}: Cannot have a null connection string");
        }

        public MandaraEntities ProductsDb()
        {
            return ProductsDb(String.Empty);
        }

        public MandaraEntities ProductsDb(string callerId)
        {
            return ContextCreator.ProductsDbConnection(_connectionString, callerId);
        }
    }
}