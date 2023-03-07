﻿using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;

// from https://github.com/reactiveui/ReactiveUI

namespace Mandara.Business.PublishSubscribe
{
    public static class PubSub
    {
        /// <summary>
        /// Listen provides an Observable that will fire whenever a Message is
        /// provided for this object via RegisterMessageSource or SendMessage.
        /// </summary>
        /// <typeparam name="T">The type of the message to listen to.</typeparam>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <returns>An Observable representing the notifications posted to the
        /// message bus.</returns>
        public static IObservable<T> Listen<T>(string contract = null)
        {
            return MessageBus.Current.Listen<T>(contract);
        }

        /// <summary>
        /// Sends a single message using the specified Type and contract.
        /// Consider using RegisterMessageSource instead if you will be sending
        /// messages in response to other changes such as property changes
        /// or events.
        /// </summary>
        /// <typeparam name="T">The type of the message to send.</typeparam>
        /// <param name="message">The actual message to send</param>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        public static void SendMessage<T>(T message, string contract = null)
        {
            MessageBus.Current.SendMessage<T>(message, contract);
        }

        /// <summary>
        /// MessageBus represents an object that can act as a "Message Bus", a
        /// simple way for ViewModels and other objects to communicate with each
        /// other in a loosely coupled way.
        ///
        /// Specifying which messages go where is done via a combination of the Type
        /// of the message as well as an additional "Contract" parameter; this is a
        /// unique string used to distinguish between messages of the same Type, and
        /// is arbitrarily set by the client. 
        /// </summary>
        private class MessageBus : IMessageBus
        {
            private readonly Dictionary<Tuple<Type, string>, NotAWeakReference> messageBus =
                new Dictionary<Tuple<Type, string>, NotAWeakReference>();

            private readonly IDictionary<Tuple<Type, string>, IScheduler> schedulerMappings =
                new Dictionary<Tuple<Type, string>, IScheduler>();

            private MessageBus()
            {
            }

            /// <summary>
            /// Registers a scheduler for the type, which may be specified at runtime, and the contract.
            /// </summary>
            /// <remarks>If a scheduler is already registered for the specified runtime and contract, this will overwrite the existing registration.</remarks>
            /// <typeparam name="T">The type of the message to listen to.</typeparam>
            /// <param name="scheduler">The scheduler on which to post the
            /// notifications for the specified type and contract. RxApp.DeferredScheduler by default.</param>
            /// <param name="contract">A unique string to distinguish messages with
            /// identical types (i.e. "MyCoolViewModel") - if the message type is
            /// only used for one purpose, leave this as null.</param>
            public void RegisterScheduler<T>(IScheduler scheduler, string contract = null)
            {
                schedulerMappings[new Tuple<Type, string>(typeof (T), contract)] = scheduler;
            }

            /// <summary>
            /// Listen provides an Observable that will fire whenever a Message is
            /// provided for this object via RegisterMessageSource or SendMessage.
            /// </summary>
            /// <typeparam name="T">The type of the message to listen to.</typeparam>
            /// <param name="contract">A unique string to distinguish messages with
            /// identical types (i.e. "MyCoolViewModel") - if the message type is
            /// only used for one purpose, leave this as null.</param>
            /// <returns>An Observable representing the notifications posted to the
            /// message bus.</returns>
            public IObservable<T> Listen<T>(string contract = null)
            {
                //            this.Log().Info("Listening to {0}:{1}", typeof (T), contract);

                return SetupSubjectIfNecessary<T>(contract);
            }

            /// <summary>
            /// Determines if a particular message Type is registered.
            /// </summary>
            /// <param name="type">The Type of the message to listen to.</param>
            /// <param name="contract">A unique string to distinguish messages with
            /// identical types (i.e. "MyCoolViewModel") - if the message type is
            /// only used for one purpose, leave this as null.</param>
            /// <returns>True if messages have been posted for this message Type.</returns>
            public bool IsRegistered(Type type, string contract = null)
            {
                bool ret = false;
                WithMessageBus(type, contract, (mb, tuple) => { ret = mb.ContainsKey(tuple) && mb[tuple].IsAlive; });

                return ret;
            }

            /// <summary>
            /// Registers an Observable representing the stream of messages to send.
            /// Another part of the code can then call Listen to retrieve this
            /// Observable.
            /// </summary>
            /// <typeparam name="T">The type of the message to listen to.</typeparam>
            /// <param name="source">An Observable that will be subscribed to, and a
            /// message sent out for each value provided.</param>
            /// <param name="contract">A unique string to distinguish messages with
            /// identical types (i.e. "MyCoolViewModel") - if the message type is
            /// only used for one purpose, leave this as null.</param>
            public IDisposable RegisterMessageSource<T>(
                IObservable<T> source,
                string contract = null)
            {
                return source.Subscribe(SetupSubjectIfNecessary<T>(contract));
            }

            /// <summary>
            /// Sends a single message using the specified Type and contract.
            /// Consider using RegisterMessageSource instead if you will be sending
            /// messages in response to other changes such as property changes
            /// or events.
            /// </summary>
            /// <typeparam name="T">The type of the message to send.</typeparam>
            /// <param name="message">The actual message to send</param>
            /// <param name="contract">A unique string to distinguish messages with
            /// identical types (i.e. "MyCoolViewModel") - if the message type is
            /// only used for one purpose, leave this as null.</param>
            public void SendMessage<T>(T message, string contract = null)
            {
                SetupSubjectIfNecessary<T>(contract).OnNext(message);
            }

            // lazy singleton
            private static Lazy<MessageBus> instance = new Lazy<MessageBus>(() => new MessageBus());

            /// <summary>
            /// Returns the Current MessageBus instance.
            /// </summary>
            public static MessageBus Current
            {
                get { return instance.Value; }
            }

            private ISubject<T> SetupSubjectIfNecessary<T>(string contract)
            {
                ISubject<T> ret = null;

                WithMessageBus(typeof (T), contract, (mb, tuple) =>
                                                         {
                                                             NotAWeakReference subjRef;
                                                             if (mb.TryGetValue(tuple, out subjRef) && subjRef.IsAlive)
                                                             {
                                                                 ret = (ISubject<T>) subjRef.Target;
                                                                 return;
                                                             }

                                                             ret = new ScheduledSubject<T>(GetScheduler(tuple));
                                                             mb[tuple] = new NotAWeakReference(ret);
                                                         });

                return ret;
            }

            private void WithMessageBus(
                Type type,
                string contract,
                Action<Dictionary<Tuple<Type, string>, NotAWeakReference>,
                    Tuple<Type, string>> block)
            {
                lock (messageBus)
                {
                    var tuple = new Tuple<Type, String>(type, contract);
                    block(messageBus, tuple);
                    if (messageBus.ContainsKey(tuple) && !messageBus[tuple].IsAlive)
                    {
                        messageBus.Remove(tuple);
                    }
                }
            }

            private IScheduler GetScheduler(Tuple<Type, string> tuple)
            {
                IScheduler scheduler;
                schedulerMappings.TryGetValue(tuple, out scheduler);
                return scheduler ?? Scheduler.Default;
            }


            internal class NotAWeakReference
            {
                public NotAWeakReference(object target)
                {
                    Target = target;
                }

                public object Target { get; private set; }

                public bool IsAlive
                {
                    get { return true; }
                }
            }

        }
    }
}