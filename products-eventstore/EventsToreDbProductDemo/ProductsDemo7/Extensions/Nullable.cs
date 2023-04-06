using DevExpress.XtraSpreadsheet.Model.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsDemo7.Extensions
{
    internal static class Nullable
    {

        public static Guid? toGuidNullable
            (this string? value)
        {
            if(String.IsNullOrEmpty(value))
            {
                return null;
            }
            if(Guid.TryParse(value, out var valueGuid))
            {
                return valueGuid;
            }
            return null;
        }
    }
}
