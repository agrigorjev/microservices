using System;
using System.Collections.Generic;

namespace Mandara.Business.DataInterface
{
    /// <summary>
    /// Provides a method to get prices timestamps for a day.
    /// </summary>
    public interface IPricesTimestampsRepository
    {
        /// <summary>
        /// Gets most recent business day end of day datetime for which a timestamp in a price database exist.
        /// Usually it should be previous business day end of day datetime, but if, for some reason, it doesn't
        /// exist, we will go back till we find one.
        /// </summary>
        /// <returns>Most recent business day end of day datetime.</returns>
        DateTime GetMostRecentBusinessDaySnapshotDatetime();

        /// <summary>
        /// Gets prices timestamps for a specified day, if the snapshotDay today it only read new timestamp not cached already
        /// </summary>
        /// <param name="snapshotDay">A day for which to get prices timestamps.</param>
        /// <returns>A list of prices timestamps.</returns>
        List<TimeSpan> GetPriceTimestamps(DateTime snapshotDay);
    }
}