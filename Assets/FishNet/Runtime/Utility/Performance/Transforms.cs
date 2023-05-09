using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace FishNet.Utility.Performance
{
    public static class GetNonAlloc
    {
        /// <summary>
        /// </summary>
        private static readonly List<Transform> _transformList = new();

        /// <summary>
        /// </summary>
        private static readonly List<NetworkBehaviour> _networkBehavioursList = new();

        /// <summary>
        ///     Gets all NetworkBehaviours on a transform.
        /// </summary>
        public static List<NetworkBehaviour> GetNetworkBehaviours(this Transform t)
        {
            t.GetComponents(_networkBehavioursList);
            return _networkBehavioursList;
        }

        /// <summary>
        ///     Gets all transforms on transform and it's children.
        /// </summary>
        public static List<Transform> GetTransformsInChildrenNonAlloc(this Transform t, bool includeInactive = false)
        {
            t.GetComponentsInChildren(includeInactive, _transformList);
            return _transformList;
        }
    }
}