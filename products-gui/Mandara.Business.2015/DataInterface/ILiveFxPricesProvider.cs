namespace Mandara.Business.DataInterface
{
    public interface ILiveFxPricesProvider : IFxPricesProvider
    {
        void UpdatePrices();
        ILiveFxPricesProvider GetFixedLivePrices();
    }
}