using JetBrains.Annotations;
using Mandara.Date;
using Newtonsoft.Json;
using System;

namespace Mandara.Business.Bus.Messages.Reporting
{
    [Serializable]
    public class ReportParams
    {
        public string Name { get; }
        public DateRange Dates { get; }

        public const string DefaultName = "";

        public static ReportParams Default = new ReportParams(DefaultName, DateRange.Default);

        [JsonConstructor]
        public ReportParams([NotNull] string name, [NotNull] DateRange dates)
        {
            Name = name ?? DefaultName;
            Dates = dates ?? DateRange.Default;
        }

        public bool IsDefault()
        {
            return DefaultName == Name
                   && DateRange.Default.Equals(Dates);
        }
    }
}
