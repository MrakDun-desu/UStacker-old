using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FishNet.Managing.Object;
using FishNet.Object;
using FishNet.Utility.Extension;
using UnityEngine;

namespace FishNet.Utility.Performance
{
    public class DefaultObjectPool : ObjectPool
    {
        #region Serialized.

        /// <summary>
        ///     True if to use object pooling.
        /// </summary>
        [Tooltip("True if to use object pooling.")] [SerializeField]
        private bool _enabled = true;

        #endregion

        #region Private.

        /// <summary>
        ///     Current count of the cache collection.
        /// </summary>
        private int _cacheCount;

        #endregion

        /// <summary>
        ///     Returns an object that has been stored with a collectionId of 0. A new object will be created if no stored objects
        ///     are available.
        /// </summary>
        /// <param name="prefabId">PrefabId of the object to return.</param>
        /// <param name="asServer">True if being called on the server side.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override NetworkObject RetrieveObject(int prefabId, bool asServer)
        {
            return RetrieveObject(prefabId, 0, asServer);
        }

        /// <summary>
        ///     Returns an object that has been stored. A new object will be created if no stored objects are available.
        /// </summary>
        /// <param name="prefabId">PrefabId of the object to return.</param>
        /// <param name="collectionId">CollectionId of the prefab.</param>
        /// <param name="asServer">True if being called on the server side.</param>
        /// <returns></returns>
        public override NetworkObject RetrieveObject(int prefabId, ushort collectionId, bool asServer)
        {
            var po = NetworkManager.GetPrefabObjects<PrefabObjects>(collectionId, false);
            //Quick exit/normal retrieval when not using pooling.
            if (!_enabled)
            {
                var prefab = po.GetObject(asServer, prefabId);
                return Instantiate(prefab);
            }

            var cache = GetOrCreateCache(collectionId, prefabId);
            NetworkObject nob;
            //Iterate until nob is populated just in case cache entries have been destroyed.
            do
            {
                if (cache.Count == 0)
                {
                    var prefab = po.GetObject(asServer, prefabId);
                    /* A null nob should never be returned from spawnables. This means something
                     * else broke, likely unrelated to the object pool. */
                    nob = Instantiate(prefab);
                    //Can break instantly since we know nob is not null.
                    break;
                }

                nob = cache.Pop();
            } while (nob == null);

            nob.gameObject.SetActive(true);
            return nob;
        }

        /// <summary>
        ///     Stores an object into the pool.
        /// </summary>
        /// <param name="instantiated">Object to store.</param>
        /// <param name="asServer">True if being called on the server side.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void StoreObject(NetworkObject instantiated, bool asServer)
        {
            //Pooling is not enabled.
            if (!_enabled)
            {
                Destroy(instantiated.gameObject);
                return;
            }

            instantiated.gameObject.SetActive(false);
            instantiated.ResetForObjectPool();
            var cache = GetOrCreateCache(instantiated.SpawnableCollectionId, instantiated.PrefabId);
            cache.Push(instantiated);
        }

        /// <summary>
        ///     Instantiates a number of objects and adds them to the pool.
        /// </summary>
        /// <param name="prefab">Prefab to cache.</param>
        /// <param name="count">Quantity to spawn.</param>
        /// <param name="asServer">
        ///     True if storing prefabs for the server collection. This is only applicable when using
        ///     DualPrefabObjects.
        /// </param>
        public void CacheObjects(NetworkObject prefab, int count, bool asServer)
        {
            if (!_enabled)
                return;
            if (count <= 0)
                return;
            if (prefab == null)
                return;
            if (prefab.PrefabId == NetworkObject.UNSET_PREFABID_VALUE)
            {
                InstanceFinder.NetworkManager.LogError(
                    $"Pefab {prefab.name} has an invalid prefabId and cannot be cached.");
                return;
            }

            var cache = GetOrCreateCache(prefab.SpawnableCollectionId, prefab.PrefabId);
            for (var i = 0; i < count; i++)
            {
                var nob = Instantiate(prefab);
                nob.gameObject.SetActive(false);
                cache.Push(nob);
            }
        }

        /// <summary>
        ///     Clears pools for all collectionIds
        /// </summary>
        public void ClearPool()
        {
            var count = _cache.Count;
            for (var i = 0; i < count; i++)
                ClearPool(i);
        }

        /// <summary>
        ///     Clears a pool for collectionId.
        /// </summary>
        /// <param name="collectionId">CollectionId to clear for.</param>
        public void ClearPool(int collectionId)
        {
            if (collectionId >= _cacheCount)
                return;

            var dict = _cache[collectionId];
            //Convert to a list from the stack so we do not modify the stack directly.
            var nobCache = ListCaches.GetNetworkObjectCache();
            foreach (var item in dict.Values)
                while (item.Count > 0)
                    nobCache.AddValue(item.Pop());
        }


        /// <summary>
        ///     Gets a cache for an id or creates one if does not exist.
        /// </summary>
        /// <param name="prefabId"></param>
        /// <returns></returns>
        private Stack<NetworkObject> GetOrCreateCache(int collectionId, int prefabId)
        {
            if (collectionId >= _cacheCount)
            {
                //Add more to the cache.
                while (_cache.Count <= collectionId)
                {
                    var dict = new Dictionary<int, Stack<NetworkObject>>();
                    _cache.Add(dict);
                }

                _cacheCount = collectionId;
            }

            var dictionary = _cache[collectionId];
            Stack<NetworkObject> cache;
            //No cache for prefabId yet, make one.
            if (!dictionary.TryGetValueIL2CPP(prefabId, out cache))
            {
                cache = new Stack<NetworkObject>();
                dictionary[prefabId] = cache;
            }

            return cache;
        }

        #region Public.

        /// <summary>
        ///     Cache for pooled NetworkObjects.
        /// </summary>
        public IReadOnlyCollection<Dictionary<int, Stack<NetworkObject>>> Cache => _cache;

        private readonly List<Dictionary<int, Stack<NetworkObject>>> _cache = new();

        #endregion
    }
}