using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Extensions.Option;

namespace Mandara.Business
{
    public class SequenceQueue<T> where T : class, IMessage
    {
        private readonly ConcurrentPriorityQueue<int, T> _queue = new ConcurrentPriorityQueue<int, T>();
        private const int InvalidSequenceNumber = 0;
        private int _lastDequeuedSequenceNumber = InvalidSequenceNumber;
        private readonly object _lockQueue = new object();
        //private bool _pauseDequeuing;

        public TryGetVal<int> LastDequeuedSequence => new TryGetVal<int>((val) => val == InvalidSequenceNumber)
        {
            Value = _lastDequeuedSequenceNumber
        };

        public List<int> ExistingSequences => _queue.Select(it => it.Key).ToList();

        public void Enqueue(T item)
        {
            lock (_lockQueue)
            {
                if (item.SequenceNumber <= _lastDequeuedSequenceNumber)
                {
                    return;
                }

                _queue.Enqueue(item.SequenceNumber, item);
                Monitor.PulseAll(_lockQueue);
            }
        }

        public TryGetResult<T> TryDequeue()
        {
            return TryDequeueWithLock();
        }

        public TryGetResult<T> TryDequeue(TimeSpan maxWaitTime)
        {
            lock (_lockQueue)
            {
                TryGetResult<T> result = TryDequeueInternal();

                if (result.HasValue)
                {
                    return result;
                }

                bool waitSuccess = Monitor.Wait(_lockQueue, maxWaitTime);

                if (waitSuccess)
                {
                    //We already have the lock so call the one without locks
                    result = TryDequeueInternal();
                }

                return result;
            }
        }

        public void ResetSequence()
        {
            lock (_lockQueue)
            {
                _queue.Clear();
                _lastDequeuedSequenceNumber = InvalidSequenceNumber;
            }
        }

        /// <summary>
        ///     This method acquire the lock first and dequeu the first element from the queue (if exisist)
        /// </summary>
        /// <returns></returns>
        private TryGetResult<T> TryDequeueWithLock()
        {
            lock (_lockQueue)
            {
                return TryDequeueInternal();
            }
        }

        /// <summary>This method can be called after the lock already taken</summary>
        /// <returns></returns>
        private TryGetResult<T> TryDequeueInternal()
        {
            T result = default(T);
            TryGetResult<KeyValuePair<int, T>> queueHeadResult = GetNextElementInSequence();

            if (!queueHeadResult.HasValue)
            {
                return new TryGetRef<T>((val) => true);
            }

            KeyValuePair<int, T> element;

            if (_queue.TryDequeue(out element))
            {
                _lastDequeuedSequenceNumber = element.Key;
                return new TryGetRef<T>() { Value = element.Value };
            }

            return new TryGetRef<T>() { Value = result };
        }

        private TryGetResult<KeyValuePair<int, T>> GetNextElementInSequence()
        {
            KeyValuePair<int, T> element;
            int expectedSequenceNumber = GetNextExpectedSequenceNumber();

            // There is no element or only the first element has a higher key than expected
            if (!_queue.TryPeek(out element) || !IsSequenceNumberValid(element.Key, expectedSequenceNumber))
            {
                return new TryGetVal<KeyValuePair<int, T>>((val) => true);
            }

            return new TryGetVal<KeyValuePair<int, T>>() { Value = element };
        }

        private bool IsSequenceNumberValid(int sequenceNumber, int baseSequenceNumber)
        {
            return InvalidSequenceNumber == baseSequenceNumber || (sequenceNumber <= baseSequenceNumber);
        }

        private int GetNextExpectedSequenceNumber()
        {
            return InvalidSequenceNumber != _lastDequeuedSequenceNumber
                ? _lastDequeuedSequenceNumber + 1
                : InvalidSequenceNumber;
        }
    }
}