namespace Mandara.TradeApiService.Data
{
    public static class NullableBooleanExtensions
    {
        public static bool True(this bool? nullableBool)
        {
            if (nullableBool.HasValue)
            {
                return nullableBool.Value;
            }

            return false;
        }

        public static bool False(this bool? nullableBool)
        {
            return !nullableBool.True();
        }
    }

}
