using System;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Serializing;
using UnityEngine;

namespace FishNet.Connection
{
    /// <summary>
    ///     A container for a connected client used to perform actions on and gather information for the declared client.
    /// </summary>
    public partial class NetworkConnection : IEquatable<NetworkConnection>
    {
        private readonly List<Vector3> _objectsPositionsCache = new();

        /// <summary>
        ///     Level of detail for each NetworkObject.
        /// </summary>
        public Dictionary<NetworkObject, byte> LevelOfDetails = new();

        /* REALLY REALLY REALLY IMPORTANT.
         * Do not let the client exceed MTU. This would
         * result in a check if limit client MTU is enabled.
         * Or perhaps allow this packet specifically to exceed MTU. 
         * 
         * Connection would likely not exceed MTU unless they had many hundred
         * objects visible to them, but better safe than sorry. */

        internal void SendLevelOfDetails()
        {
            if (!IsActive)
                return;
            if (!IsLocalClient)
                return;

            //Rebuild position cache for players objects.
            _objectsPositionsCache.Clear();
            foreach (var playerObjects in Objects)
                _objectsPositionsCache.Add(playerObjects.transform.position);

            var pw = WriterPool.GetWriter(5000);

            var spawned = NetworkManager.ClientManager.Objects.Spawned;
            foreach (var nob in spawned.Values)
            {
                var nobPosition = nob.transform.position;
                var closestDistance = float.MaxValue;
                foreach (var objPosition in _objectsPositionsCache)
                {
                    var dist = Vector3.SqrMagnitude(nobPosition - objPosition);
                    if (dist < closestDistance)
                        closestDistance = dist;

                    byte lod;
                    if (dist <= 10 * 10)
                        lod = 0;
                    else if (dist <= 20 * 20)
                        lod = 1;
                    else if (dist <= 40 * 40)
                        lod = 2;
                    else
                        lod = 3;

                    pw.WriteNetworkObjectId(nob.ObjectId);
                    pw.WriteByte(lod);
                }
            }
        }
    }
}