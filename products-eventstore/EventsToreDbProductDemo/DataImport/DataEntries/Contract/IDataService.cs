using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImport.DataEntries.Contract
{
    internal interface IDataService<T>
    {
        List<T> GetDataList();
    }
}
