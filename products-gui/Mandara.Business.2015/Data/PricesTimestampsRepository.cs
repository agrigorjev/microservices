using Mandara.Business.DataInterface;
using Mandara.Business.Dates;
using Mandara.Database.Query;
using Mandara.Database.Sql;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Extensions.Option;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Optional;

namespace Mandara.Business.Data
{
    public class PricesTimestampsRepository : IPricesTimestampsRepository
    {
        private readonly IEndOfDayDateTimeProvider _endOfDaysDateTimes;
        private readonly ILogger _log = new NLogLoggerFactory().GetCurrentClassLogger();
        private readonly ConcurrentDictionary<DateTime, List<TimeSpan>> _timespanCache = new ConcurrentDictionary<DateTime, List<TimeSpan>>();
        private readonly Comparer<TimeSpan> _timeStampComparer = Comparer<TimeSpan>.Default;
        private static readonly SqlConnectionStringBuilder PriceDbConnStr;

        static PricesTimestampsRepository()
        {
            PriceDbConnStr = ConnectionString.GetConnectionStringBuild("PriceDatabase");
        }

        public PricesTimestampsRepository(IEndOfDayDateTimeProvider endOfDaysDateTimes)
        {
            _endOfDaysDateTimes = endOfDaysDateTimes;
        }

        public DateTime GetMostRecentBusinessDaySnapshotDatetime()
        {
            DateTime prevSnapshotDay = SystemTime.Today();
            DateTime snapshotDatetime = DateTime.MinValue;

            do
            {
                prevSnapshotDay = prevSnapshotDay.GetPreviousBusinessDay();

                Option<DateTime> eodTime = _endOfDaysDateTimes.Eod(prevSnapshotDay);
                List<TimeSpan> priceTimestamps = GetPriceTimestamps(prevSnapshotDay);

                if (priceTimestamps.Count > 0)
                {
                    snapshotDatetime = SetSnapshotDateTimeFromTimeStamps(eodTime, priceTimestamps, prevSnapshotDay);
                }
            }
            while (snapshotDatetime == DateTime.MinValue);

            return snapshotDatetime;
        }

        private DateTime SetSnapshotDateTimeFromTimeStamps(
            Option<DateTime> eodTime,
            List<TimeSpan> priceTimestamps,
            DateTime prevSnapshotDay)
        {
            return eodTime.Match(
                eod => TimestampWithEodTimeExist(priceTimestamps, eod) ? eod : FallbackTimeStamp(prevSnapshotDay),
                () => FallbackTimeStamp(prevSnapshotDay));

            DateTime FallbackTimeStamp(DateTime snapshotDay)
            {
                DateTime snapshotTime = GetPriceTimestampCloseTo1800(priceTimestamps, snapshotDay);

                _log.Warn(
                    "TradesRepository set SnapshotDateTime: Timestamps were found for {0}.  Could not find an end of "
                        + "day time or the value found did not match any stored timestamps.  Using timestamp closest "
                        + "to 18h00: {1}",
                    snapshotDay,
                    snapshotTime);
                return snapshotTime;
            }
        }

        private static bool TimestampWithEodTimeExist(List<TimeSpan> priceTimestamps, DateTime eodTime)
        {
            return priceTimestamps.Any(x => x == eodTime.TimeOfDay);
        }

        private static DateTime GetPriceTimestampCloseTo1800(List<TimeSpan> priceTimestamps, DateTime prevSnapshotDay)
        {
            TimeSpan snapshotTime = new TimeSpan(18, 00, 00);
            TimeSpan minTimespan = priceTimestamps.Min();

            int deltaMinutes = 1;
            if (minTimespan > snapshotTime)
            {
                deltaMinutes = -1;
            }

            do
            {
                if (priceTimestamps.Contains(snapshotTime))
                {
                    return prevSnapshotDay.Add(snapshotTime);
                }

                snapshotTime = snapshotTime.Subtract(TimeSpan.FromMinutes(deltaMinutes));
            }
            while (snapshotTime.Subtract(minTimespan) > TimeSpan.FromMinutes(1));

            snapshotTime = minTimespan;

            return prevSnapshotDay.Add(snapshotTime);
        }

        public List<TimeSpan> GetPriceTimestamps(DateTime snapshotDay)
        {
            if (!_timespanCache.ContainsKey(snapshotDay))
            {
                int priceTimestamp = EpochConverter.ToEpochTime(snapshotDay);

                List<TimeSpan> timestamps = SqlServerCommandExecution.ReadToList(
                    PriceDbConnStr,
                    ConstructSqlForDayCommand(priceTimestamp),
                    ConstructResultFromRow(priceTimestamp),
                    _timeStampComparer);

                _timespanCache[snapshotDay] = timestamps;
            }
            else if (snapshotDay == SystemTime.Today())
            {
                List<TimeSpan> todayTimestamps = _timespanCache[snapshotDay];
                lock (todayTimestamps)
                {

                    DateTime lastTimestamp = SystemTime.Today();
                    if (_timespanCache[snapshotDay].Any())
                    {
                        lastTimestamp = lastTimestamp.AddMilliseconds(_timespanCache[snapshotDay].Max(x => x).TotalMilliseconds);
                    }

                    List<TimeSpan> newTimeStamps = GetNewTimestamps(lastTimestamp);
                    todayTimestamps.AddRange(newTimeStamps);
                }
            }

            return _timespanCache[snapshotDay];
        }

        private List<TimeSpan> GetNewTimestamps(DateTime lastTimestamp)
        {
            int lastSdateTimeStamp = EpochConverter.ToEpochTime(lastTimestamp);
            int dateStartTimestamp = EpochConverter.ToEpochTime(lastTimestamp.Date);

            List<TimeSpan> timestamps = SqlServerCommandExecution.ReadToList(
                PriceDbConnStr,
                ConstructNewTimestampCommand(lastSdateTimeStamp),
                ConstructResultFromRow(dateStartTimestamp),
                _timeStampComparer);

            return timestamps;
        }

        private Func<SqlDataReader, TimeSpan> ConstructResultFromRow(int priceTimestamp)
        {
            return reader =>
            {
                int seconds = Convert.ToInt32(reader[0]) - priceTimestamp;
                return new TimeSpan(0, 0, seconds);
            };
        }

        private Func<SqlConnection, SqlCommand> ConstructSqlForDayCommand(int priceTimestamp)
        {
            return conn =>
            {
                string commandText = "SELECT DISTINCT sdate FROM dbo.mandara_dumps WITH (NOLOCK)"
                                     + "WHERE sdate between @timestamp and (@timestamp + 3600 * 24) ";
                SqlCommand comm = new SqlCommand(commandText, conn);

                comm.Parameters.Add(new SqlParameter("@timestamp", priceTimestamp));
                comm.CommandTimeout = 0;

                return comm;
            };
        }

        private Func<SqlConnection, SqlCommand> ConstructNewTimestampCommand(int priceTimestamp)
        {
            return conn =>
            {
                string commandText = "SELECT DISTINCT sdate FROM dbo.mandara_today_sdates WITH (NOLOCK)"
                                     + "WHERE sdate > @timestamp ";
                SqlCommand comm = new SqlCommand(commandText, conn);

                comm.Parameters.Add(new SqlParameter("@timestamp", priceTimestamp));
                comm.CommandTimeout = 0;

                return comm;
            };
        }
    }
}