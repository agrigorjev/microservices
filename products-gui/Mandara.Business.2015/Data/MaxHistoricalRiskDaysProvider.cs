using Mandara.Extensions.Option;
using System;
using System.Configuration;
using Mandara.Extensions.AppSettings;

namespace Mandara.Business.Data
{
    /// <summary>
    /// Provides MaxHistoricalRiskDays property to get a number of days in the past that is
    /// allowed for a live trades form.
    /// </summary>
    public static class MaxHistoricalRiskDaysProvider
    {
        private const int MinRiskDays = 30;
        private const int MaxRiskDays = 1100;
        private static int _maxHistoricalDays = Int32.MinValue;

        /// <summary>
        /// Get a maximum number of days a live trades form is allowed to select in the past.
        /// Defined by an optional "MaxHistoricalRiskDays" app setting, default value is 30,
        /// minimum - 30, maximum - 1100.
        /// </summary>
        public static int MaxHistoricalRiskDays
        {
            get
            {
                if (Int32.MinValue != _maxHistoricalDays)
                {
                    return _maxHistoricalDays;
                }

                TryGetResult<string> maxHistoricalRiskDays =
                    ConfigurationManager.AppSettings.TryGet("MaxHistoricalRiskDays");

                int numDays;

                if (maxHistoricalRiskDays.HasValue && int.TryParse(maxHistoricalRiskDays.Value, out numDays))
                {
                    if (numDays < MinRiskDays)
                    {
                        _maxHistoricalDays = MinRiskDays;
                    }
                    else if (numDays > MaxRiskDays)
                    {
                        _maxHistoricalDays = MaxRiskDays;
                    }
                    else
                    {
                        _maxHistoricalDays = numDays;
                    }
                }
                else
                {
                    _maxHistoricalDays = MinRiskDays;
                }

                return _maxHistoricalDays;
            }
        }
    }
}