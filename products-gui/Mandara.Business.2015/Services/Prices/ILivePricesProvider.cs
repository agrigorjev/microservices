namespace Mandara.Business.Services.Prices
{
    public interface ILivePricesProvider : IPricesProvider
    {
        IPricesProvider GetFixedLivePrices();

        void UpdatePrices();
    }
}