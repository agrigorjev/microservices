using Mandara.Date;
using Mandara.Extensions.Option;
using System;
using Optional;

namespace Mandara.Business.Dates
{
    /// <summary>
    /// Works with end of day datetimes. Provides methods to get eod datetime periods or the dates themselfs.
    /// </summary>
    public interface IEndOfDayDateTimeProvider
    {
        /// <summary>
        /// Gets date range in UTC timezone for a current live trades constraints.
        /// </summary>
        /// <returns>DateRange with dates in UTC timezone.</returns>
        DateRange GetLiveUtcEodDateRange();

        /// <summary>
        /// Gets previous business day end of day datetime. It would skip weekends and dates without
        /// end of day datetime set. 
        /// </summary>
        /// <returns>End of day datetime if one was found.</returns>
        TryGetResult<DateTime> PrevBusinessDayEndOfDay(DateTime day);

        /// <summary>
        /// Gets end of day datetimes for a passed DateRange dates and converts them to UTC.
        /// </summary>
        /// <returns>DateRange with dates in UTC timezone.</returns>
        DateRange GetUtcDateRangeAccordingToEodTimes(DateRange periodInDays);

        /// <summary>
        /// Gets end of day datetime for a specified day.
        /// </summary>
        /// <param name="dayForEod">A day we need to lookup an end of day datetime for.</param>
        /// <returns>End of day datetime if one was found.</returns>
        Option<DateTime> Eod(DateTime dayForEod);

        /// <summary>
        /// Gets end of day datetime for a specified day.
        /// </summary>
        /// <param name="dayForEod">A day we need to lookup an end of day datetime for.</param>
        /// <returns>End of day datetime if one was found.</returns>
        [Obsolete]
        TryGetResult<DateTime> TryGetEod(DateTime dayForEod);

        TryGetResult<DateTime> GetPrevBusinessDayEodDatetimeUtc(DateTime previousBusinessDay);
        Option<DateTime> GetPrevBusinessDayEodUtc(DateTime previousBusinessDay);
    }
}