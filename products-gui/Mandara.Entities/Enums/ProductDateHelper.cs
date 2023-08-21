using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mandara.Entities
{
    public static class ProductDateHelper
    {
        public static DateTime DateFromQuarter(int year, int quarter)
        {
            return new DateTime(year, 1 + 3 * (quarter - 1), 1);
        }

        public static DateTime DateFromYear(int year)
        {
            return new DateTime(year, 1, 1);
        }

        public static DateTime DateFromMonth(int year, int month)
        {
            return new DateTime(year, month, 1);
        }
    }
}
