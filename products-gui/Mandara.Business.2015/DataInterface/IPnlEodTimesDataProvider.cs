using System;
using System.Collections.Generic;

namespace Mandara.Business.DataInterface
{
    /// <summary>
    /// Provides methods to get and add end of day datetimes.
    /// </summary>
    public interface IPnlEodTimesDataProvider
    {
        /// <summary>
        /// Gets a number of end of day datetimes.
        /// </summary>
        /// <param name="numberOfDaysToLoad">A number of end of day datetimes to get.</param>
        /// <returns>A dictionary where a key is a day, and a value - it's end of day datetime.</returns>
        Dictionary<DateTime, DateTime> GetPnlEodTimes(int numberOfDaysToLoad);

        /// <summary>
        /// Sets end of day time for a day.
        /// </summary>
        /// <param name="snapshotDatetime">An en of day time and a day to set it for.</param>
        void SetPnlEodTime(DateTime snapshotDatetime);
    }
}