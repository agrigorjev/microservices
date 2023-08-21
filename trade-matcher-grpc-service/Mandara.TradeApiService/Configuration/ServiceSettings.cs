namespace Mandara.TradeApiService.Configuration;

public class ServiceSettings
{
    public const string SectionName = "ConfigurationServiceSettings";
    public int PollingIntervalMilliseconds { get; set; }
    public List<int> IsDailyProductTypes { get; set; }
    public ServiceSettings()
    {
        PollingIntervalMilliseconds = 1000 * 60 * 5;
        IsDailyProductTypes = new List<int> { 8, 9, 12, 13 };
    }
}
