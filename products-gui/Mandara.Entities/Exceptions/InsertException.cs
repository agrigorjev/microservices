using System;
using System.Data.SqlClient;

namespace Mandara.Entities.Exceptions
{
    public class SecDefPrecalcInsertFailure
    {
        public int SecurityDef { get; }
        public int Product { get; }
        public DateTime Month { get; }

        public SecDefPrecalcInsertFailure(string[] keyFields)
        {
            SecurityDef = Int32.Parse(keyFields[0]);
            Product = Int32.Parse(keyFields[1]);
            Month = DateTime.Parse(keyFields[2]);
        }
    }

    public class TradePrecalcInsertFailure
    {
        public int Trade { get; }
        public int Product { get; }
        public DateTime MinDay { get; }
        public DateTime MaxDay { get; }

        public TradePrecalcInsertFailure(string[] keyFields)
        {
            Trade = Int32.Parse(keyFields[0]);
            Product = Int32.Parse(keyFields[1]);
            MinDay = DateTime.Parse(keyFields[2]);
            MaxDay = DateTime.Parse(keyFields[3]);
        }
    }

    public enum InsertFailureSource
    {
        TradePrecalc,
        SecurityPrecalc,
    }

    public class InsertException<T> : Exception where T : class
    {
        public InsertFailureSource Table { get; }
        public string[] RowKey { get; }
        public T FailedEntry { get; }

        public InsertException(
            string msg,
            InsertFailureSource table,
            string[] rowKey,
            T failedEntry,
            SqlException sourceException) : base(msg, sourceException)
        {
            Table = table;
            RowKey = rowKey;
            FailedEntry = failedEntry;
        }
    }
}
