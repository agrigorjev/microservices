using Mandara.Business.Data;
using Mandara.Business.DataInterface;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Extensions.Option;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Optional.Unsafe;

namespace Mandara.Business.Dates
{
    public class EndOfDayDateTimeProvider : IEndOfDayDateTimeProvider
    {
        private readonly IPnlEodTimesDataProvider _endOfDaySource;
        private Dictionary<DateTime, DateTime> _endOfDayTimesByDate;
        private DateTime _minEndOfDayDateTime = InternalTime.LocalToday();

        public EndOfDayDateTimeProvider(IPnlEodTimesDataProvider pnlEodDataProvider)
        {
            _endOfDaySource = pnlEodDataProvider;
        }

        public Option<DateTime> Eod(DateTime dayForEod)
        {
            return EndOfDayByDate().TryGetValue(dayForEod.Date, out DateTime eod)
                ? Option.Some(eod)
                : Option.None<DateTime>();
        }

        private Dictionary<DateTime, DateTime> EndOfDayByDate()
        {
            if (null != _endOfDayTimesByDate)
            {
                return _endOfDayTimesByDate;
            }

            ILogger log = new NLogLoggerFactory().GetCurrentClassLogger();

            log.Debug(
                "Reading end of day times for previous {0} days",
                MaxHistoricalRiskDaysProvider.MaxHistoricalRiskDays);
            _endOfDayTimesByDate = _endOfDaySource.GetPnlEodTimes(MaxHistoricalRiskDaysProvider.MaxHistoricalRiskDays);
            _minEndOfDayDateTime =
                _endOfDayTimesByDate.Any() ? _endOfDayTimesByDate.Keys.Min() : InternalTime.LocalToday();
            log.Debug(
                "Read {0} end of day times with minimum {1}",
                _endOfDayTimesByDate.Count,
                _minEndOfDayDateTime.ToShortDateAndTime());

            return _endOfDayTimesByDate;
        }

        [Obsolete("Call Eod")]
        public TryGetResult<DateTime> TryGetEod(DateTime dayForEod)
        {
            return Eod(dayForEod)
                .Match(
                    eod => new TryGetVal<DateTime>(val => false) { Value = eod },
                    () => new TryGetVal<DateTime>(val => true));
        }

        private readonly HashSet<DateTime> _previousEodMisses = new HashSet<DateTime>();

        public Option<DateTime> GetPrevBusinessDayEod(DateTime day)
        {
            ILogger log = new NLogLoggerFactory().GetCurrentClassLogger();
            (DateTime eodDatetime, DateTime prevNonWeekend) = SearchForEndOfDay(log, day.Date.PreviousBusinessDay());

            _previousEodMisses.Remove(prevNonWeekend);
            log.Trace(
                "End of day for {0}, is {1}",
                prevNonWeekend.ToShortDateAndTime(),
                eodDatetime.ToShortDateAndTime());

            return eodDatetime > DateTime.MinValue && _minEndOfDayDateTime != eodDatetime
                ? Option.Some(eodDatetime)
                : Option.None<DateTime>();
        }

        private (DateTime endOfDay, DateTime endingDay) SearchForEndOfDay(ILogger log, DateTime endingDay)
        {
            DateTime eodDatetime = _minEndOfDayDateTime;

            while (!EndOfDayByDate().TryGetValue(endingDay, out eodDatetime)
                   && endingDay >= _minEndOfDayDateTime)
            {
                if (!_previousEodMisses.Contains(endingDay))
                {
                    _previousEodMisses.Add(endingDay);
                    log.Debug("Didn't find end of day for {0}", endingDay.ToShortDateAndTime());
                }

                endingDay = endingDay.PreviousBusinessDay();
            }

            return (eodDatetime, endingDay);
        }

        public TryGetResult<DateTime> GetPrevBusinessDayEodDatetimeUtc(DateTime day)
        {
            return ConvertToUtc(PrevBusinessDayEndOfDay(day));
        }

        [Obsolete]
        public TryGetResult<DateTime> PrevBusinessDayEndOfDay(DateTime day)
        {
            return new TryGetVal<DateTime>(GetPrevBusinessDayEod(day.Date).ValueOr(_minEndOfDayDateTime));
        }

        public Option<DateTime> GetPrevBusinessDayEodUtc(DateTime day)
        {
            return ConvertToUtc(GetPrevBusinessDayEod(day));
        }

        public DateRange GetUtcDateRangeAccordingToEodTimes(DateRange periodInDays)
        {
            TryGetResult<DateTime> startEodDate = new TryGetVal<DateTime>();
            TryGetResult<DateTime> endEodDate = new TryGetVal<DateTime>();

            if (periodInDays.HasStartDate())
            {
                startEodDate = GetStartDateEndOfDay(periodInDays.Start);
            }

            if (periodInDays.HasEndDate())
            {
                endEodDate = GetEndDateEndOfDay(periodInDays, startEodDate);
            }

            return new DateRange(startEodDate, endEodDate);
        }

        private TryGetResult<DateTime> GetStartDateEndOfDay(DateTime startDate)
        {
            return ConvertToUtc(PrevBusinessDayEndOfDay(startDate));
        }

        private TryGetResult<DateTime> GetEndDateEndOfDay(
            DateRange periodInDays,
            TryGetResult<DateTime> startEodDate)
        {
            TryGetResult<DateTime> eodForEndDate;

            if (periodInDays.End.Date <= SystemTime.Today())
            {
                eodForEndDate = PrevBusinessDayEndOfDay(periodInDays.End.AddDays(1));
            }
            else
            {
                eodForEndDate = new TryGetVal<DateTime> { Value = periodInDays.End };
            }

            return ConvertToUtc(
                eodForEndDate.Equals(startEodDate) ? new TryGetVal<DateTime>(periodInDays.End) : eodForEndDate);
        }

        public DateRange GetLiveUtcEodDateRange()
        {
            DateTime today = SystemTime.Today();

            return GetUtcDateRangeAccordingToEodTimes(new DateRange(today, today.AddDays(1)));
        }

        [Obsolete]
        private TryGetResult<DateTime> ConvertToUtc(TryGetResult<DateTime> time)
        {
            if (time.HasValue)
            {
                return new TryGetVal<DateTime> { Value = time.Value.ToUniversalTime() };
            }

            return time;
        }

        private Option<DateTime> ConvertToUtc(Option<DateTime> time)
        {
            return time.HasValue ? Option.Some(time.ValueOrFailure().ToUniversalTime()) : time;
        }
    }
}
