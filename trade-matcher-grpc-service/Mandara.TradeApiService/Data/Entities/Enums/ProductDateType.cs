namespace Mandara.TradeApiService.Data;

public enum ProductDateType
{
    NotSet = -1,
    MonthYear = 0,
    Year = 1,
    Day = 2,
    Quarter = 3,
    Custom = 4,
    Daily = 5
}

public static class ProductDateTypeExtensions
{
    public static int GetMonthsForDateType(this ProductDateType dateType)
    {
        int months;

        switch (dateType)
        {
            case ProductDateType.Year:
                {
                    months = 12;
                }
                break;

            case ProductDateType.Quarter:
                {
                    months = 3;
                }
                break;

            default:
                {
                    months = 1;
                }
                break;
        }

        return months;
    }
}
