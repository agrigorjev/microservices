using com.latencybusters.lbm;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Mandara.Extensions.Collections;

namespace Mandara.Business.Bus
{
    public partial class InformaticaHelper
    {
        public static readonly Dictionary<string, LBMSource> Sources = new Dictionary<string, LBMSource>();
        public static ConcurrentBag<LBMRequest> TempRequests = new ConcurrentBag<LBMRequest>();

        public virtual void CreateSources()
        {
        }

        public void CreateSources(List<string> customSources)
        {
            CreateSources();
            customSources.Where(src => !Sources.ContainsKey(src)).ForEach(
                src =>
                {
                    AddSource(src);
                });
        }

        protected virtual void AddSource(string topicName)
        {
            if (!Sources.ContainsKey(topicName))
            {
                LBMTopic topic = LbmContext.allocTopic(topicName);
                LBMSource source = LbmContext.createSource(topic);
                Sources.Add(topicName, source);
            }
        }

        public virtual LBMSource GetSource(string topicName)
        {
            LBMSource source;

            if (Sources.TryGetValue(topicName, out source))
            {
                return source;
            }

            return null;
        }

        public virtual void CloseSources()
        {
            foreach (KeyValuePair<string, LBMSource> pair in Sources)
            {
                if ((pair.Value != null) && !pair.Value.isClosed())
                {
                    pair.Value.close();
                }
            }

            Sources.Clear();

            LBMRequest request;

            while (TempRequests.TryTake(out request))
            {
                if (request == null)
                {
                    continue;
                }

                request.close();
            }
        }
    }
}
