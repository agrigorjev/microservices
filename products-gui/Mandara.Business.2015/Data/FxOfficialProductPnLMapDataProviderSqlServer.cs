using Mandara.Business.DataInterface;
using Mandara.Entities;
using Mandara.Entities.Entities;
using Mandara.Entities.Extensions;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Mandara.Business.Data
{
    public class FxOfficialProductPnLMapDataProviderSqlServer : IFxOfficialProductPnLMapDataProvider
    {
        private readonly ILogger _log;

        public FxOfficialProductPnLMapDataProviderSqlServer(ILogger log)
        {
            _log = log;
        }

        public List<FxOfficialProductPnLMap> GetFxOfficialProductPnLMaps()
        {
            try
            {
                using (MandaraEntities dbContext = new MandaraEntities(
                    MandaraEntities.DefaultConnStrName,
                    nameof(FxOfficialProductPnLMapDataProviderSqlServer)))
                {
                    return dbContext.FxOfficialProductPnLMaps.ToList();
                }
            }
            catch (EntityCommandExecutionException entityCmdExecException) when (
                entityCmdExecException.InnerException is SqlException sqlReadErr)
            {
                HandleSqlException(sqlReadErr);
            }
            catch (SqlException sqlReadErr)
            {
                HandleSqlException(sqlReadErr);
            }

            return new List<FxOfficialProductPnLMap>();
        }

        private void HandleSqlException(SqlException sqlEx)
        {
            StringBuilder sqlErrors = new StringBuilder();

            foreach (SqlError sqlErr in sqlEx.Errors)
            {
                sqlErrors.AppendFormat(
                    "Error number = {0}, Error message = {1}, Error state = {2}, Error class = {3}\n",
                    sqlErr.Number,
                    sqlErr.Message,
                    sqlErr.State,
                    sqlErr.Class);
            }

            _log.Error(
                sqlEx,
                "FxOfficialProductPnLMapDataProviderSqlServer: Could not read the FxOfficialProductPnLMap "
                    + "table - \n{0}",
                sqlErrors.ToString());
        }
    }
}
