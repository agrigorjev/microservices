using com.latencybusters.lbm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Mandara.Extensions.Collections;

namespace Mandara.Business.Bus
{
    public partial class InformaticaHelper
    {
        public static readonly Dictionary<string, LBMReceiver> Receivers = new Dictionary<string, LBMReceiver>();
        public static ConcurrentDictionary<LBMReceiver, bool> TempReceivers =
            new ConcurrentDictionary<LBMReceiver, bool>();

        public virtual void CreateReceivers()
        {
        }

        public void CreateReceivers(List<Tuple<string, Type>> customReceivers)
        {
            CreateReceivers();
            customReceivers.Where(recv => !Receivers.ContainsKey(recv.Item1)).ForEach(
                recv =>
                {
                    AddReceiver(recv.Item1, recv.Item2);
                });
        }

        protected virtual void AddReceiver(string topicName, Type handlerType)
        {
            if (!Receivers.ContainsKey(topicName))
            {
                LBMTopic topic = LbmContext.lookupTopic(topicName);
                LBMReceiver receiver = LbmContext.createReceiver(topic, OnMessageReceive, null, null);
                Receivers.Add(topicName, receiver);
                HandlerManager.RegisterHandler(topicName, handlerType);
            }
        }

        public virtual void CloseReceivers()
        {
            foreach (KeyValuePair<string, LBMReceiver> pair in Receivers)
            {
                if (pair.Value != null)
                {
                    pair.Value.close();
                }
            }

            Receivers.Clear();

            foreach (LBMReceiver receiver in TempReceivers.Keys.ToList())
            {
                if (receiver != null)
                {
                    try
                    {
                        receiver.close();
                    }
                    catch
                    {
                    }
                }

                TempReceivers.TryRemove(receiver, out bool _);
            }
        }
    }
}
