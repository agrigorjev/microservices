using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandara.Entities;

namespace Mandara.Business.Managers
{
    public static class CurrencyManager
    {
        private static IEnumerable<Currency> _currencies;

        public static IEnumerable<Currency> Currencies
        {
            get { return _currencies; }
        }

        public static void Init(IEnumerable<Currency> currencies)
        {
            if (_currencies == null)
                _currencies = currencies ?? new List<Currency>();
        }

    }
}
