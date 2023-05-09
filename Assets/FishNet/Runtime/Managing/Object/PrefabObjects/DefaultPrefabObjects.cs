using System.Collections.Generic;
using FishNet.Documenting;
using FishNet.Object;
using FishNet.Object.Helping;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FishNet.Managing.Object
{
    [APIExclude]
    //[CreateAssetMenu(fileName = "New DefaultPrefabObjects", menuName = "FishNet/Spawnable Prefabs/Default Prefab Objects")]
    public class DefaultPrefabObjects : SinglePrefabObjects
    {
        /// <summary>
        ///     Sets asset path hashes for prefabs starting at index, or if missing.
        /// </summary
        /// <return>Returns true if one or more NetworkObjects were updated.</return>
        internal bool SetAssetPathHashes(int index)
        {
#if UNITY_EDITOR
            var dirtied = false;
            var count = base.GetObjectCount();

            if (count == 0)
                return false;
            if (index < 0 || index >= count)
            {
                Debug.LogError(
                    $"Index {index} is out of range when trying to set asset path hashes. Collection length is {count}. Defaulf prefabs may need to be rebuilt.");
                return false;
            }

            for (var i = 0; i < count; i++)
            {
                var n = Prefabs[i];
                if (i < index)
                    continue;

                var pathAndName = $"{AssetDatabase.GetAssetPath(n.gameObject)}{n.gameObject.name}";
                var hashcode = pathAndName.GetStableHash64();
                //Already set.
                if (n.AssetPathHash == hashcode)
                    continue;

                n.SetAssetPathHash(hashcode);
                EditorUtility.SetDirty(n);
                dirtied = true;
            }

            return dirtied;
#else
            return false;
#endif
        }

        /// <summary>
        ///     Sorts prefabs by name and path hashcode.
        /// </summary>
        internal void Sort()
        {
            if (base.GetObjectCount() == 0)
                return;

            var hashcodesAndNobs = new Dictionary<ulong, NetworkObject>();
            var hashcodes = new List<ulong>();

            var error = false;
            foreach (var n in Prefabs)
            {
                hashcodes.Add(n.AssetPathHash);
                //If hashcode is 0 something is wrong
                if (n.AssetPathHash == 0)
                {
                    error = true;
                    Debug.LogError($"AssetPathHash is not set for GameObject {n.name}.");
                }

                hashcodesAndNobs.Add(n.AssetPathHash, n);
            }

            //An error occured, no reason to continue.
            if (error)
            {
                Debug.LogError(
                    "One or more NetworkObject prefabs did not have their AssetPathHash set. This usually occurs when a prefab cannot be saved. Check the specified prefabs for missing scripts or serialization errors and correct them, then use Fish-Networking -> Refresh Default Prefabs.");
                return;
            }

            //Once all hashes have been made re-add them to prefabs sorted.
            hashcodes.Sort();
            //Build to a new list using sorted hashcodes.
            var sortedNobs = new List<NetworkObject>();
            foreach (var hc in hashcodes)
                sortedNobs.Add(hashcodesAndNobs[hc]);

            base.Clear();
            base.AddObjects(sortedNobs);
        }
    }
}