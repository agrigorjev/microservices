using Mandara.Entities.FxExposure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mandara.Business.Bus.Messages.Fx
{
    public class FxHedgeDetailMessage : FxSnapshotMessageBase
    {
        public int PortfolioId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int CurrencyId { get; private set; }
        private List<FxHedgeDetailEntry> _fxHedgeDetailEntries;

        public const int MaximumDaysInDateRange = 730;

        public IEnumerable<FxHedgeDetailEntry> FxHedgeDetailData
        {
            get { return _fxHedgeDetailEntries; }
            private set
            {
                _fxHedgeDetailEntries = value.ToList();
            }
        }

        [JsonConstructor]
        public FxHedgeDetailMessage(
            int portfolioId,
            int currencyId,
            DateTime startDate,
            DateTime endDate,
            IEnumerable<FxHedgeDetailEntry> fxHedgeDetailData)
        {
            FxHedgeDetailData = fxHedgeDetailData ?? new List<FxHedgeDetailEntry>();
            PortfolioId = portfolioId;
            CurrencyId = currencyId;
            StartDate = startDate;
            EndDate = endDate;
            Success = true;
        }

        public FxHedgeDetailMessage(
            int portfolioId, 
            int currencyId, 
            DateTime startDate, 
            DateTime endDate)
            : this(portfolioId, currencyId, startDate, endDate, new List<FxHedgeDetailEntry>())
        {
            ValidateArguments(portfolioId, currencyId, startDate, endDate, FxHedgeDetailData);
        }

        private void ValidateArguments(
            int portfolioId,
            int currencyId,
            DateTime startDate,
            DateTime endDate,
            IEnumerable<FxHedgeDetailEntry> fxHedgeEntries)
        {
            ValidateParameter<int>(IsValidDBTableId, portfolioId, "The portfolio ID must be an integer greater than 0.");
            ValidateParameter(IsValidDBTableId, currencyId, "The currency ID must be an integer greater than 0.");
            ValidateParameter(
                IsValidStartAndEndDatePair,
                new Tuple<DateTime, DateTime>(startDate, endDate),
                String.Format(
                    "The start and end date must be valid date/time values, the start date cannot be after the end "
                        + "date and the number of days must be fewer than {0}.", FxHedgeDetailMessage.MaximumDaysInDateRange));
            ValidateParameter(
                IsValidFxHedgeDetailCollection,
                fxHedgeEntries,
                "The FX hedge details collection cannot be null or contain null entries.");
        }

        private bool IsValidDBTableId(int id)
        {
            return id >= 1 && id != Int32.MaxValue;
        }

        private bool IsValidStartAndEndDatePair(Tuple<DateTime, DateTime> startAndDate)
        {
            DateTime startDate = startAndDate.Item1;
            DateTime endDate = startAndDate.Item2;
            return IsValidDateTime(startDate)
                && IsValidDateTime(endDate)
                && startDate <= endDate
                && (endDate - startDate).TotalDays <= FxHedgeDetailMessage.MaximumDaysInDateRange;
        }

        private bool IsValidDateTime(DateTime dateTimeToTest)
        {
            return dateTimeToTest != DateTime.MinValue && dateTimeToTest != DateTime.MaxValue;
        }

        private bool IsValidFxHedgeDetailCollection(IEnumerable<FxHedgeDetailEntry> fxHedgeDetailEntries)
        {
            return null != fxHedgeDetailEntries && !fxHedgeDetailEntries.Any(entry => null == entry);
        }

        public void AddFxHedgeDetailEntry(FxHedgeDetailEntry fxHedgeDetailEntry)
        {
            if (fxHedgeDetailEntry == null)
            {
                throw new ArgumentNullException("fxHedgeDetailEntry");
            }

            _fxHedgeDetailEntries.Add(fxHedgeDetailEntry);
        }

        public void AddFxHedgeDetailEntryRange(IEnumerable<FxHedgeDetailEntry> fxHedgeDetailEntryRange)
        {
            ValidateParameter(
                IsValidFxHedgeDetailCollection,
                fxHedgeDetailEntryRange,
                "The FX hedge details collection cannot be null or contain null entries.");

            _fxHedgeDetailEntries.AddRange(fxHedgeDetailEntryRange);
        }

        public override void OnErrorSet()
        {
            _fxHedgeDetailEntries.Clear();
        }

        public override string ToString()
        {
            StringBuilder fxHedgeAsString = new StringBuilder();

            fxHedgeAsString.AppendFormat(
                "{0} -- {1} -- {2} -- {3}",
                PortfolioId,
                CurrencyId,
                StartDate,
                EndDate);
            return
                FxHedgeDetailData.Aggregate(
                    fxHedgeAsString,
                    (hedgeEntries, entry) => hedgeEntries.AppendFormat("\n{0}", entry)).ToString();
        }
    }
}
