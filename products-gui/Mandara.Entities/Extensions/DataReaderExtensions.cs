using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Mandara.Entities.Extensions
{
    public static class DataReaderExtensions
    {
        public static bool IsDBNull(this IDataReader instance,  string columnName )
        {
            return instance[columnName] == DBNull.Value;
        }

        public static int GetInt(this IDataReader instance, string columnName)
        {
            return Convert.ToInt32(instance[columnName]);
        }
        
        public static decimal? GetDecimal(this IDataReader instance, string columnName)
        {
            if (instance.IsDBNull(columnName))
            {
                return null;
            }
            return Convert.ToDecimal(instance[columnName]);
        }
    }
}
