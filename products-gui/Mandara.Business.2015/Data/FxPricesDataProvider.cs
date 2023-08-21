using Mandara.Business.DataInterface;
using Mandara.Database.Query;
using Mandara.Database.Sql;
using Mandara.Entities;
using Mandara.Extensions.Option;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Mandara.Date;

namespace Mandara.Business.Data
{
    public class FxPricesDataProvider : IFxPricesDataProvider
    {
        private readonly ILogger _log;
        private static readonly SqlConnectionStringBuilder PriceDbConnStr;
        public ICurrencyProvider CurrencyProvider { get; private set; }

        static FxPricesDataProvider()
        {
            PriceDbConnStr = ConnectionString.GetConnectionStringBuild("PriceDatabase");
        }

        public FxPricesDataProvider(ICurrencyProvider currProvider, ILogger log)
        {
            CurrencyProvider = currProvider;
            _log = log;
        }

        public virtual Dictionary<string, decimal> GetLivePrices()
        {
            return GetPrices("mandara_fx_live", DateTime.MinValue);
        }

        public virtual Dictionary<string, decimal> GetSnapshotPrices(DateTime snapshotDateTime)
        {
            return GetPrices("mandara_fx", snapshotDateTime);
        }

        private Dictionary<string, decimal> GetPrices(string tableName, DateTime snapshotDateTime)
        {
            Dictionary<string, decimal> prices = new Dictionary<string, decimal>();

            List<FxPriceData> fxPrices = ReadFxPrices(tableName, snapshotDateTime);

            if (fxPrices.Count == 0)
            {
                return prices;
            }

            UpdateCurrencyProvider(fxPrices);

            foreach (FxPriceData fxPrice in fxPrices)
            {
                TryGetResult<CurrencyPair> currencyPair = CurrencyProvider.TryGetCurrencyPair(fxPrice.PairId);

                if (!currencyPair.HasValue)
                {
                    _log.Warn(
                        "FxPricesDataProvider: CurrencyPair not found when parsing {0} table, price {1}",
                        tableName,
                        fxPrice);
                    continue;
                }

                string fxKey = FxPriceKey.Create(fxPrice.ValueDate, currencyPair.Value);

                decimal storedPrice;
                if (prices.TryGetValue(fxKey, out storedPrice))
                {
                    _log.Warn(
                        "FxPricesDataProvider: already had a price for ValueDate [{0}] and a CurrencyPair [{1}] "
                        + "of [{2}] but trying to insert another price [{3}], will use the first price. "
                        + "This could be due to a two similar pairs defined, e.g. USD/GBP and GBP/USD while "
                        + "only one should exist.",
                        tableName,
                        currencyPair.ToString(),
                        storedPrice,
                        fxPrice.Rate);
                }
                else
                {
                    prices.Add(fxKey, fxPrice.Rate);
                }
            }

            return prices;
        }

        private void UpdateCurrencyProvider(List<FxPriceData> fxPrices)
        {
            List<int> pairsIdFromPricesDb = fxPrices.Select(x => x.PairId).Distinct().ToList();

            CurrencyProvider.Update(pairsIdFromPricesDb);
        }

        private List<FxPriceData> ReadFxPrices(string tableName, DateTime snapshotDateTime)
        {
            try
            {
                List<FxPriceData> fxPrices = SqlServerCommandExecution.ReadToList(
                    PriceDbConnStr,
                    conn => ComposeCommand(conn, tableName, snapshotDateTime),
                    dr =>
                        new FxPriceData
                        {
                            Timestamp = dr.GetDateTime(0),
                            ValueDate = dr.GetDateTime(1),
                            PairId = dr.GetInt32(2),
                            Rate = Convert.ToDecimal(dr.GetDouble(3))
                        });

                return fxPrices;
            }
            catch (SqlException sqlEx)
            {
                HandleSqlException(tableName, sqlEx);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "FxPricesDataProvider: error reading prices from {0} table", tableName);
            }

            return new List<FxPriceData>();
        }

        private SqlCommand ComposeCommand(SqlConnection connection, string tableName, DateTime snapshotDateTime)
        {
            SqlCommand command;
            string commandTextBase = string.Format("select TimeStamp, ValueDate, RateId, Rate from {0} ", tableName);

            if (snapshotDateTime != DateTime.MinValue)
            {
                string commandText = commandTextBase + " where TimeStamp = @timestamp";
                command = new SqlCommand(commandText, connection);
                command.Parameters.Add(new SqlParameter("@timestamp", snapshotDateTime));
            }
            else
            {
                command = new SqlCommand(commandTextBase, connection);
            }

            return command;
        }

        private void HandleSqlException(string tableName, SqlException sqlEx)
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
                "FxPricesDataProvider: Could not read fx prices from table {0}: \n{1}",
                tableName,
                sqlErrors.ToString());
        }


        private class FxPriceData
        {
            public DateTime Timestamp { get; set; }
            public DateTime ValueDate { get; set; }
            public int PairId { get; set; }
            public decimal Rate { get; set; }

            public override string ToString()
            {
                return string.Format(
                    "Timestamp=[{0}], ValueDate=[{1}], PairId=[{2}], Rate=[{3}]",
                    Timestamp.ToShortDateAndTime(),
                    ValueDate.ToSortableShortDate('-'),
                    PairId,
                    Rate);
            }
        }
    }
}