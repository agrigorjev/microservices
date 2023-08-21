using System.Collections.Generic;

namespace Mandara.Business.Data
{
    internal class TestPhasePriceDump
    {
        // TODO: test method, remove it before code goes live
        public static void DumpPrices(string tempFilePath, Dictionary<string, decimal> prices)
        {
            return;
            //File.WriteAllText(tempFilePath, "Key,Price\n");
            //using (StreamWriter streamWriter = new StreamWriter(tempFilePath, true))
            //using (CsvWriter csv = new CsvWriter(streamWriter))
            //{
            //    foreach (KeyValuePair<string, decimal> pair in prices)
            //    {
            //        csv.WriteField(pair.Key);
            //        csv.WriteField(pair.Value);

            //        csv.NextRecord();
            //    }
            //}
        }
    }
}
