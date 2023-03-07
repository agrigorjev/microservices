using System;
using Ninject.Extensions.Logging;

namespace Mandara.Business.AsyncServices.Base
{
    public static class AsyncServiceFactory
    {
        public static AsyncService FromAction(AsyncService.CancellableAction action, ILogger log)
        {
            return new AsyncService.CancelActionAsyncService(action, log);
        }

        public static AsyncService FromAction(Action action, ILogger log)
        {
            return new AsyncService.ActionAsyncService(action, log);
        }
    }
}