using System;
using System.Collections.Generic;

namespace Mandara.Business.AsyncServices.Base
{
    public class AsyncServiceManager : IDisposable
    {
        private readonly List<AsyncService> _services = new List<AsyncService>();

        public void AddService(AsyncService service)
        {
            _services.Add(service);
        }

        public void AddRange(IEnumerable<AsyncService> services)
        {
            _services.AddRange(services);
        }

        public void Start()
        {
            _services.ForEach(s => s.Start());
        }

        public void Stop()
        {
            _services.ForEach(s => s.Stop());
        }

        public void Dispose()
        {
            _services.ForEach(svc => svc.Dispose());
        }
    }
}
