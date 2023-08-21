using Mandara.Entities.Enums;
using System;

namespace Mandara.Entities.Calculation
{
    [Serializable]
    public class OvernightPnlCalculationDetail : PnlCalculationDetail
    {
        public OvernightPnlMessageType MessageType { get; set; }
        public string Source { get; set; }
        public string StripName { get; set; }
        public Money OvernightAmount { get; set; }

        private const OvernightPnlMessageType DefaultType = OvernightPnlMessageType.SnapshotEnd;
        private const string DefaultSource = "NoSource";
        private const string DefaultStrip = "NoStrip";
        private static readonly Money DefaultAmount = Money.Default;

        public static readonly OvernightPnlCalculationDetail Default = new OvernightPnlCalculationDetail()
        {
            MessageType = DefaultType,
            Source = DefaultSource,
            StripName = DefaultStrip,
            OvernightAmount = DefaultAmount,
        };

        public bool IsDefault()
        {
            return DefaultType == MessageType
                   && DefaultSource == Source
                   && DefaultStrip == StripName
                   && DefaultAmount == OvernightAmount;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Source)}: {Source}, {nameof(StripName)}: {StripName}, {nameof(OvernightAmount)}: {OvernightAmount} Calculation: {base.ToString()}";
        }
    }
}