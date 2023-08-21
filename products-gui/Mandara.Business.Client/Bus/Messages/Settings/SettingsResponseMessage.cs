using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Settings
{
    public class SettingsResponseMessage : MessageBase
    {
        public int MaxHistoricalRiskDays { get; set; }
    }
}