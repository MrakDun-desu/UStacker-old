using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace FishNet.Utility.Performance
{
    /// <summary>
    ///     Various ListCache instances that may be used on the Unity thread.
    /// </summary>
    public static class ListCaches
    {
        /// <summary>
        ///     Cache collection for NetworkObjects.
        /// </summary>
        private static readonly Stack<ListCache<NetworkObject>> _networkObjectCaches = new();

        /// <summary>
        ///     Cache collection for NetworkObjects.
        /// </summary>
        private static readonly Stack<ListCache<NetworkBehaviour>> _networkBehaviourCaches = new();

        /// <summary>
        ///     Cache collection for NetworkObjects.
        /// </summary>
        private static readonly Stack<ListCache<Transform>> _transformCaches = new();

        /// <summary>
        ///     Cache collection for NetworkConnections.
        /// </summary>
        private static readonly Stack<ListCache<NetworkConnection>> _networkConnectionCaches = new();

        /// <summary>
        ///     Cache collection for ints.
        /// </summary>
        private static readonly Stack<ListCache<int>> _intCaches = new();


        #region GetCache.

        /// <summary>
        ///     Returns a NetworkObject cache. Use StoreCache to return the cache.
        /// </summary>
        /// <returns></returns>
        public static ListCache<NetworkObject> GetNetworkObjectCache()
        {
            ListCache<NetworkObject> result;
            if (_networkObjectCaches.Count == 0)
                result = new ListCache<NetworkObject>();
            else
                result = _networkObjectCaches.Pop();

            return result;
        }

        /// <summary>
        ///     Returns a NetworkConnection cache. Use StoreCache to return the cache.
        /// </summary>
        /// <returns></returns>
        public static ListCache<NetworkConnection> GetNetworkConnectionCache()
        {
            ListCache<NetworkConnection> result;
            if (_networkConnectionCaches.Count == 0)
                result = new ListCache<NetworkConnection>();
            else
                result = _networkConnectionCaches.Pop();

            return result;
        }

        /// <summary>
        ///     Returns a Transform cache. Use StoreCache to return the cache.
        /// </summary>
        /// <returns></returns>
        public static ListCache<Transform> GetTransformCache()
        {
            ListCache<Transform> result;
            if (_transformCaches.Count == 0)
                result = new ListCache<Transform>();
            else
                result = _transformCaches.Pop();

            return result;
        }

        /// <summary>
        ///     Returns a NetworkBehaviour cache. Use StoreCache to return the cache.
        /// </summary>
        /// <returns></returns>
        public static ListCache<NetworkBehaviour> GetNetworkBehaviourCache()
        {
            ListCache<NetworkBehaviour> result;
            if (_networkBehaviourCaches.Count == 0)
                result = new ListCache<NetworkBehaviour>();
            else
                result = _networkBehaviourCaches.Pop();

            return result;
        }

        /// <summary>
        ///     Returns an int cache. Use StoreCache to return the cache.
        /// </summary>
        /// <returns></returns>
        public static ListCache<int> GetIntCache()
        {
            ListCache<int> result;
            if (_intCaches.Count == 0)
                result = new ListCache<int>();
            else
                result = _intCaches.Pop();

            return result;
        }

        #endregion

        #region StoreCache.

        /// <summary>
        ///     Stores a NetworkObject cache.
        /// </summary>
        /// <param name="cache"></param>
        public static void StoreCache(ListCache<NetworkObject> cache)
        {
            cache.Reset();
            _networkObjectCaches.Push(cache);
        }

        /// <summary>
        ///     Stores a NetworkConnection cache.
        /// </summary>
        /// <param name="cache"></param>
        public static void StoreCache(ListCache<NetworkConnection> cache)
        {
            cache.Reset();
            _networkConnectionCaches.Push(cache);
        }

        /// <summary>
        ///     Stores a Transform cache.
        /// </summary>
        /// <param name="cache"></param>
        public static void StoreCache(ListCache<Transform> cache)
        {
            cache.Reset();
            _transformCaches.Push(cache);
        }

        /// <summary>
        ///     Stores a NetworkBehaviour cache.
        /// </summary>
        /// <param name="cache"></param>
        public static void StoreCache(ListCache<NetworkBehaviour> cache)
        {
            cache.Reset();
            _networkBehaviourCaches.Push(cache);
        }

        /// <summary>
        ///     Stores an int cache.
        /// </summary>
        /// <param name="cache"></param>
        public static void StoreCache(ListCache<int> cache)
        {
            cache.Reset();
            _intCaches.Push(cache);
        }

        #endregion
    }

    /// <summary>
    ///     Creates a reusable cache of T which auto expands.
    /// </summary>
    public class ListCache<T>
    {
        #region Private.

        /// <summary>
        ///     Cache for type.
        /// </summary>
        private readonly Stack<T> _cache = new();

        #endregion

        public ListCache()
        {
            Collection = new List<T>();
        }

        public ListCache(int capacity)
        {
            Collection = new List<T>(capacity);
        }

        /// <summary>
        ///     Returns T from cache when possible, or creates a new object when not.
        /// </summary>
        /// <returns></returns>
        private T Retrieve()
        {
            if (_cache.Count > 0)
                return _cache.Pop();
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        ///     Stores value into the cache.
        /// </summary>
        /// <param name="value"></param>
        private void Store(T value)
        {
            _cache.Push(value);
        }

        /// <summary>
        ///     Adds a new value to Collection and returns it.
        /// </summary>
        /// <param name="value"></param>
        public T AddReference()
        {
            var next = Retrieve();
            Collection.Add(next);
            return next;
        }

        /// <summary>
        ///     Inserts an bject into Collection and returns it.
        /// </summary>
        /// <param name="value"></param>
        public T InsertReference(int index)
        {
            //Would just be at the end anyway.
            if (index >= Collection.Count)
                return AddReference();

            var next = Retrieve();
            Collection.Insert(index, next);
            return next;
        }

        /// <summary>
        ///     Adds value to Collection.
        /// </summary>
        /// <param name="value"></param>
        public void AddValue(T value)
        {
            Collection.Add(value);
        }

        /// <summary>
        ///     Inserts value into Collection.
        /// </summary>
        /// <param name="value"></param>
        public void InsertValue(int index, T value)
        {
            //Would just be at the end anyway.
            if (index >= Collection.Count)
                AddValue(value);
            else
                Collection.Insert(index, value);
        }

        /// <summary>
        ///     Adds values to Collection.
        /// </summary>
        /// <param name="values"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddValues(ListCache<T> values)
        {
            var w = values.Written;
            var c = values.Collection;
            for (var i = 0; i < w; i++)
                AddValue(c[i]);
        }

        /// <summary>
        ///     Adds values to Collection.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddValues(T[] values)
        {
            for (var i = 0; i < values.Length; i++)
                AddValue(values[i]);
        }

        /// <summary>
        ///     Adds values to Collection.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddValues(List<T> values)
        {
            for (var i = 0; i < values.Count; i++)
                AddValue(values[i]);
        }

        /// <summary>
        ///     Adds values to Collection.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddValues(HashSet<T> values)
        {
            foreach (var item in values)
                AddValue(item);
        }

        /// <summary>
        ///     Adds values to Collection.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddValues(ISet<T> values)
        {
            foreach (var item in values)
                AddValue(item);
        }

        /// <summary>
        ///     Adds values to Collection.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddValues(IReadOnlyCollection<T> values)
        {
            foreach (var item in values)
                AddValue(item);
        }


        /// <summary>
        ///     Resets cache.
        /// </summary>
        public void Reset()
        {
            foreach (var item in Collection)
                Store(item);
            Collection.Clear();
        }

        #region Public.

        /// <summary>
        ///     Collection cache is for.
        /// </summary>
        public List<T> Collection = new();

        /// <summary>
        ///     Entries currently written.
        /// </summary>
        public int Written => Collection.Count;

        #endregion
    }
}