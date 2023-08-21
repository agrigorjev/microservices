namespace Mandara.Business.DataInterface
{
    public interface IFixedFxPricesDataProvider : IFxPricesDataProvider
    {
        void Reset();
    }
}