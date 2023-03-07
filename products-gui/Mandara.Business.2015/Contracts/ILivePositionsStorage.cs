using Mandara.Business.Model;
using Ninject.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Mandara.Business.Contracts
{
    public interface ILivePositionsStorage
    {
        //IrmConcurrentDictionary<string, CalculationDetailModel> Positions { get; }

        void AddPosition(CalculationDetailModel newPos);

        void RemovePosition(CalculationDetailModel posToRemove);

        void ClearPositions();

        IEnumerable<CalculationDetailModel> GetPositions();
        IDictionary<string, CalculationDetailModel> GetPositionsDictionary();
        CalculationDetailModel GetPosition(string posId);
    }

    public class IrmConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly object lockObject = new object();
        private ConcurrentDictionary<TKey, TValue> internalDictionary;
        private readonly ILogger _log;

        public IrmConcurrentDictionary(ILogger log)
        {
            internalDictionary = new ConcurrentDictionary<TKey, TValue>();
            _log = log;
        }

        public IrmConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, ILogger log)
        {
            internalDictionary = new ConcurrentDictionary<TKey, TValue>(collection);
            _log = log;
        }

        public Dictionary<TKey, TValue> CloneInternalDictionary()
        {
            return new Dictionary<TKey, TValue>(internalDictionary);
        }

        // Exceptions:
        //   T:System.ArgumentNullException:
        //     key is null.
        //
        //   T:System.Collections.Generic.KeyNotFoundException:
        //     
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value of the key/value pair at the specified index.</returns>
        /// <exception cref="ArgumentNullException">key is null</exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is retrieved and key does not exist in the collection.
        /// </exception>
        public TValue this[TKey key]
        {
            get => internalDictionary[key];
            set => internalDictionary[key] = value;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            TValue _;

            return TryRemove(item.Key, out _);
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the System.Collections.Concurrent.ConcurrentDictionary`2.
        /// </summary>
        /// <returns>
        /// The number of key/value pairs contained in the System.Collections.Concurrent.ConcurrentDictionary`2.
        /// </returns>
        /// <exception cref="OverflowException">
        /// The dictionary already contains the maximum number of elements (System.Int32.MaxValue).
        /// </exception>
        public int Count => internalDictionary.Count;

        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets a value that indicates whether the System.Collections.Concurrent.ConcurrentDictionary`2 is empty.
        /// </summary>
        /// <returns>true if the System.Collections.Concurrent.ConcurrentDictionary`2 is empty</returns>
        public bool IsEmpty => internalDictionary.IsEmpty;

        /// <summary>
        /// Gets a collection containing the keys in the System.Collections.Generic.Dictionary`2.
        /// </summary>
        /// <returns>A collection of keys in the System.Collections.Generic.Dictionary`2.</returns>
        public ICollection<TKey> Keys => internalDictionary.Keys;

        /// <summary>
        /// Gets a collection that contains the values in the System.Collections.Generic.Dictionary`2.
        /// </summary>
        /// <returns>A collection that contains the values in the System.Collections.Generic.Dictionary`2.</returns>
        public ICollection<TValue> Values => internalDictionary.Values;

        /// <summary>
        /// Adds a key/value pair to the System.Collections.Concurrent.ConcurrentDictionary`2 if the key does not already
        /// exist, or updates a key/value pair in the System.Collections.Concurrent.ConcurrentDictionary`2
        /// </summary>
        /// <param name="key">The key to be added or whose value should be updated</param>
        /// <param name="addValue">The value to be added for an absent key</param>
        /// <param name="updateValueFactory">
        /// The function used to generate a new value for an existing key based on the key's existing value.
        /// </param>
        /// <returns>
        /// The new value for the key. This will be either be addValue (if the key was absent)or the result of
        /// updateValueFactory (if the key was present).
        /// </returns>
        /// <exception cref="ArgumentNullException">key or updateValueFactory is null.</exception>
        /// <exception cref="OverflowException">
        /// The dictionary already contains the maximum number of elements (System.Int32.MaxValue).
        /// </exception>
        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            lock (lockObject)
            {
                _log.Debug("Adding {0} to the cache. Current item count = {1}", addValue, internalDictionary.Count);

                TValue added = internalDictionary.AddOrUpdate(key, addValue, updateValueFactory);

                _log.Debug("Added {0} to the cache. Item count after = {1}", added, internalDictionary.Count);

                return added;
            }
        }

        /// <summary>
        /// Uses the specified functions to add a key/value pair to the
        /// System.Collections.Concurrent.ConcurrentDictionary`2 if the key does not already exist, or to update a
        /// key/value pair in the System.Collections.Concurrent.ConcurrentDictionary`2 if the key already exists.
        /// </summary>
        /// <param name="key">The key to be added or whose value should be updated</param>
        /// <param name="addValueFactory">The function used to generate a value for an absent key</param>
        /// <param name="updateValueFactory">
        /// The function used to generate a new value for an existing key based on the key's existing value.
        /// </param>
        /// <returns>
        /// The new value for the key. This will be either be the result of addValueFactory (if the key was absent) or
        /// the result of updateValueFactory (if the key was present).
        /// </returns>
        /// <exception cref="ArgumentNullException">key, addValueFactory, or updateValueFactory is null.</exception>
        /// <exception cref="OverflowException">
        /// The dictionary already contains the maximum number of elements (System.Int32.MaxValue).
        /// </exception>
        public TValue AddOrUpdate(
            TKey key,
            Func<TKey, TValue> addValueFactory,
            Func<TKey, TValue, TValue> updateValueFactory)
        {
            lock (lockObject)
            {
                _log.Debug(
                    "Adding to the cache using a func for new entries. Current item count = {0}",
                    internalDictionary.Count);

                TValue added = internalDictionary.AddOrUpdate(key, addValueFactory, updateValueFactory);

                _log.Debug("Adding {0} to the cache. Item count after = {1}", added, internalDictionary.Count);

                return added;
            }
        }

        /// <summary>
        /// Removes all keys and values from the System.Collections.Concurrent.ConcurrentDictionary`2.
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            lock (lockObject)
            {
                _log.Debug("Clearing the cache. Current item count = {0}", internalDictionary.Count);
                internalDictionary.Clear();
                _log.Debug("Cache is clear. Item count now = {0}", internalDictionary.Count);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (!internalDictionary.TryGetValue(item.Key, out TValue existingValue))
            {
                return false;
            }

            if (null == item.Value)
            {
                return null == existingValue;
            }

            return item.Value.Equals(existingValue);

        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            int currentIndex = arrayIndex;

            foreach (TKey key in internalDictionary.Keys)
            {
                array[currentIndex++] = new KeyValuePair<TKey, TValue>(key, internalDictionary[key]);
            }
        }

        /// <summary>
        /// Determines whether the System.Collections.Concurrent.ConcurrentDictionary`2 contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the System.Collections.Concurrent.ConcurrentDictionary`2.</param>
        /// <returns>
        /// true if the System.Collections.Concurrent.ConcurrentDictionary`2 contains an element with the specified key
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            return internalDictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            AddOrUpdate(key, value, (existingKey, existingValue) => value);
        }

        public bool Remove(TKey key)
        {
            TValue _;

            return TryRemove(key, out _);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the System.Collections.Concurrent.ConcurrentDictionary`2.
        /// </summary>
        /// <returns>An enumerator for the System.Collections.Concurrent.ConcurrentDictionary`2.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return internalDictionary.GetEnumerator();
        }

        /// <summary>
        /// Adds a key/value pair to the System.Collections.Concurrent.ConcurrentDictionary`2 if the key does not already
        /// exist.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value to be added, if the key does not already exist</param>
        /// <returns>
        /// The value for the key. This will be either the existing value for the key if the key is already in the
        /// dictionary, or the new value if the key was not in the dictionary.
        /// </returns>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="OverflowException">
        /// The dictionary already contains the maximum number of elements (System.Int32.MaxValue).
        /// </exception>
        public TValue GetOrAdd(TKey key, TValue value)
        {
            lock (lockObject)
            {
                _log.Debug(
                    "Get or add {0} to the cache for key {1}. Current item count = {2}",
                    value,
                    key,
                    internalDictionary.Count);

                TValue result = internalDictionary.GetOrAdd(key, value);

                _log.Debug(
                    "Got or added {0} to the cache for key {1}. Item count now = {2}",
                    result,
                    key,
                    internalDictionary.Count);
                return result;
            }
        }

        /// <summary>
        /// Adds a key/value pair to the System.Collections.Concurrent.ConcurrentDictionary`2 by using the specified
        /// function, if the key does not already exist.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="valueFactory">The function used to generate a value for the key</param>
        /// <returns>
        /// The value for the key. This will be either the existing value for the key if the key is already in the
        /// dictionary, or the new value for the key as returned by valueFactory if the key was not in the dictionary.
        /// </returns>
        /// <exception cref="ArgumentNullException">key or valueFactory is null.</exception>
        /// <exception cref="OverflowException">
        /// The dictionary already contains the maximum number of elements (System.Int32.MaxValue).
        /// </exception>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            lock (lockObject)
            {
                _log.Debug(
                    "Get or add by func to the cache for key {0}. Current item count = {1}",
                    key,
                    internalDictionary.Count);

                TValue result = internalDictionary.GetOrAdd(key, valueFactory);

                _log.Debug(
                    "Got or added {0} to the cache for key {1}. Item count now = {2}",
                    result,
                    key,
                    internalDictionary.Count);
                return result;
            }
        }

        /// <summary>
        /// Copies the key and value pairs stored in the System.Collections.Concurrent.ConcurrentDictionary`2 to a new
        /// array.
        /// </summary>
        /// <returns>
        /// A new array containing a snapshot of key and value pairs copied from the
        /// System.Collections.Concurrent.ConcurrentDictionary`2.
        /// </returns>
        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            return internalDictionary.ToArray();
        }

        /// <summary>
        /// Attempts to add the specified key and value to the System.Collections.Concurrent.ConcurrentDictionary`2.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        /// <returns>
        /// true if the key/value pair was added to the System.Collections.Concurrent.ConcurrentDictionary`2
        /// successfully, false if the key already exists.
        /// </returns>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="OverflowException">
        /// The dictionary already contains the maximum number of elements (System.Int32.MaxValue).
        /// </exception>
        public bool TryAdd(TKey key, TValue value)
        {
            lock (lockObject)
            {
                _log.Debug(
                    "Try to add {0} for key {1}. Current item count = {2}",
                    value,
                    key,
                    internalDictionary.Count);

                bool outcome = internalDictionary.TryAdd(key, value);

                _log.Debug(
                    "Result of add {0} for key {1} was {2}. Current item count = {3}",
                    value,
                    key,
                    outcome,
                    internalDictionary.Count);
                return outcome;
            }
        }

        /// <summary>
        /// Attempts to get the value associated with the specified key from the
        /// System.Collections.Concurrent.ConcurrentDictionary`2.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the object from the System.Collections.Concurrent.ConcurrentDictionary`2
        /// that has the specified key, or the default value of the type if the operation failed.
        /// </param>
        /// <returns>true if the key was found in the System.Collections.Concurrent.ConcurrentDictionary`2</returns>
        /// <exception cref="ArgumentNullException">key is null</exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return internalDictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Attempts to remove and return the value that has the specified key from the
        /// System.Collections.Concurrent.ConcurrentDictionary`2.
        /// </summary>
        /// <param name="key">The key of the element to remove and return.</param>
        /// <param name="value">
        /// When this method returns, contains the object removed from the
        /// System.Collections.Concurrent.ConcurrentDictionary`2, or the default value of the TValue type if key does not
        /// exist.
        /// </param>
        /// <returns>true if the object was removed successfully; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">key is null</exception>
        public bool TryRemove(TKey key, out TValue value)
        {
            lock (lockObject)
            {
                _log.Debug(
                    "Trying to remove value for key {0}. Current item count = {1}",
                    key,
                    internalDictionary.Count);

                bool outcome = internalDictionary.TryRemove(key, out value);

                _log.Debug(
                    "Result of trying to remove value for key {0} was {1} (the value was {2}). Current item count = {3}",
                    key,
                    outcome,
                    value,
                    internalDictionary.Count);
                return outcome;
            }
        }

        /// <summary>
        /// Compares the existing value for the specified key with a specified value, and if they are equal, updates the
        /// key with a third value.
        /// </summary>
        /// <param name="key">The key whose value is compared with comparisonValue and possibly replaced.</param>
        /// <param name="newValue">
        /// The value that replaces the value of the element that has the specified key if the comparison results in key
        /// equality.
        /// </param>
        /// <param name="comparisonValue">
        /// The value that is compared to the value of the element that has the specified
        /// </param>
        /// <returns>true if the value with key was equal to comparisonValue and was replaced with newValue</returns>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            lock (lockObject)
            {
                _log.Debug(
                    "Trying to update value for key {0} with {1} if the current value is {2}. Current item count = {3}",
                    key,
                    newValue,
                    comparisonValue,
                    internalDictionary.Count);

                bool outcome = internalDictionary.TryUpdate(key, newValue, comparisonValue);

                _log.Debug(
                    "Result of trying to update value for key {0} with {1} if the current value is {2} was {3}. Current item count = {4}",
                    key,
                    newValue,
                    comparisonValue,
                    outcome,
                    internalDictionary.Count);
                return outcome;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}