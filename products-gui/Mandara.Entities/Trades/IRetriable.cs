namespace Mandara.Entities.Trades
{
    public interface IRetriable
    {
        int RetryCounter { get; }
        void IncrementRetryCounter();
    }
}
