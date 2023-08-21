using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Mandara.Entities.Entities
{
    public class TableNameSource
    {
        public const string NoName = "NoTableName";

        public static string GetTableName(Type sourceEntity)
        {
            return (sourceEntity.GetCustomAttributes().FirstOrDefault(attr => attr is TableAttribute) as TableAttribute)
                   ?.Name
                   ?? NoName;
        }
    }
}
