using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Mandara.Entities.DatabaseContext
{
    public static class ContextCreator
    {
        public static MandaraEntities ProductsDbConnection(string connectionString)
        {
            return ProductsDbConnection(connectionString, String.Empty);
        }

        public static MandaraEntities ProductsDbConnection(string connectionString, string callerId)
        {
            DbConnection connection = new SqlConnection(connectionString);

            // By passing true to the construction the connection will be disposed when the DbContext that owns it is
            // disposed.
            return new MandaraEntities(connection, callerId, true);
        }
    }
}