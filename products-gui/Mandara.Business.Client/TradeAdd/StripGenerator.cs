using System;
using System.Collections.Generic;
using Mandara.Date.Time;

namespace Mandara.Business.TradeAdd
{
    public enum StripPeriodsToInclude
    {
        None = 0,
        QsAndCals = 1,
        Balmos = 2,
    }

    public class StripGenerator
    {
        private const int DefaultYearsInFuture = 4;

        public List<Strip> GenerateStrips()
        {
            return GenerateStrips(DefaultYearsInFuture, true, true);
        }

        public List<Strip> GenerateStrips(StripPeriodsToInclude periodsToInclude)
        {
            return GenerateStrips(
                DefaultYearsInFuture,
                periodsToInclude.HasFlag(StripPeriodsToInclude.QsAndCals),
                periodsToInclude.HasFlag(StripPeriodsToInclude.Balmos));
        }

        public List<Strip> GenerateStrips(int yearsIntoFuture, StripPeriodsToInclude periodsToInclude)
        {
            return GenerateStrips(
                yearsIntoFuture,
                periodsToInclude.HasFlag(StripPeriodsToInclude.QsAndCals),
                periodsToInclude.HasFlag(StripPeriodsToInclude.Balmos));
        }

        private List<Strip> GenerateStrips(int yearsInFuture, bool includeQAndCals, bool includeBalmoStrip)
        {
            List<Strip> strings = new List<Strip>();

            if (includeBalmoStrip)
            {
                strings.Add(new Strip
                {
                    IsBalmoStrip = true,
                    StringValue = "Balmo",
                    StartDate = SystemTime.Today()
                });
            }

            string[] monthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            int yearStart = SystemTime.Now().Year;
            int yearEnd = yearStart + yearsInFuture;


            for (int year = yearStart; year < yearEnd; year++)
            {
                int yearYY = year - 2000;

                bool isFirstYear = year == yearStart;
                bool isLastYear = yearEnd - year == 1;

                int monthStart = isFirstYear ? SystemTime.Now().Month - 1 : 0;
                int monthEnd = isLastYear ? SystemTime.Now().Month : 12;

                for (int m = monthStart; m < monthEnd; m++)
                {
                    strings.Add(new Strip
                    {
                        StringValue = monthNames[m] + yearYY,
                        StartDate = new DateTime(year, m + 1, 1)
                    });
                }

                if (includeQAndCals)
                {
                    int qStart = isFirstYear ? SystemTime.Now().Month / 4 : 0;
                    int qEnd = isLastYear ? (SystemTime.Now().Month / 4) + 1 : 4;

                    for (int q = qStart; q < qEnd; q++)
                    {
                        strings.Add(new Strip
                        {
                            StringValue = $"Q{q + 1} {yearYY}",
                            StartDate = new DateTime(year, q * 3 + 1, 1)
                        });
                    }

                    strings.Add(new Strip
                    {
                        StringValue = $"Cal {yearYY}",
                        StartDate = new DateTime(year, 1, 1)
                    });
                }
            }

            return strings;
        }
    }
}