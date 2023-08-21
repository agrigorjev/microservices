using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using Ninject.Extensions.Logging;

namespace Mandara.Business.Import.Services
{
    public interface IFxSpotRateDataProvider
    {
        decimal GetLiveSpotRate(ILogger log);
    }

    public class CsvTestDataProvider : IFxSpotRateDataProvider
    {
        public decimal GetLiveSpotRate(ILogger log)
        {
            string spotSourceFileName = "SpotRate.csv";
            decimal spotRate=110;

            try
            {
                using (TextReader fileReader = File.OpenText(spotSourceFileName))
                {
                    using (CsvReader csv = new CsvReader(fileReader, CultureInfo.InvariantCulture))
                    {
                        while (csv.Read())
                        {
                            spotRate = csv.GetField<decimal>(0);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error(exception, "Error reading value from file, returning default spot rate 110. Exception:" );
                return 110;
            }
           
            return spotRate;
        }
    }
}
