namespace Mandara.IRM.Server.Services
{
    public interface ILivePricesStorage : IPricesStorage
    {
        ILivePricesStorage GetFixedLivePrices();

        void Stop();
    }
}