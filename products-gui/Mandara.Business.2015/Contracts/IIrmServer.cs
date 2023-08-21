namespace Mandara.Business.Contracts
{
    public interface IIrmServer
    {
        void Start(object hostControl);
        void Stop();
    }
}