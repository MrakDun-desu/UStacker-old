using System;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

namespace FishNet.Managing.Server
{
    public sealed partial class ServerManager : MonoBehaviour
    {
        /// <summary>
        ///     Initializes RPC Links for NetworkBehaviours.
        /// </summary>
        private void InitializeRpcLinks()
        {
            /* Brute force enum values. 
             * Linq Last/Max lookup throws for IL2CPP. */
            ushort highestValue = 0;
            var pidValues = Enum.GetValues(typeof(PacketId));
            foreach (PacketId pid in pidValues)
                highestValue = Math.Max(highestValue, (ushort) pid);

            highestValue += 1;
            for (var i = highestValue; i < ushort.MaxValue; i++)
                _availableRpcLinkIndexes.Enqueue(i);
        }

        /// <summary>
        ///     Sets the next RPC Link to use.
        /// </summary>
        /// <returns>True if a link was available and set.</returns>
        internal bool GetRpcLink(out ushort value)
        {
            if (_availableRpcLinkIndexes.Count > 0)
            {
                value = _availableRpcLinkIndexes.Dequeue();
                return true;
            }

            value = 0;
            return false;
        }

        /// <summary>
        ///     Sets data to RpcLinks for linkIndex.
        /// </summary>
        internal void SetRpcLink(ushort linkIndex, RpcLink data)
        {
            RpcLinks[linkIndex] = data;
        }

        /// <summary>
        ///     Returns RPCLinks to availableRpcLinkIndexes.
        /// </summary>
        internal void StoreRpcLinks(Dictionary<uint, RpcLinkType> links)
        {
            foreach (var rlt in links.Values)
                _availableRpcLinkIndexes.Enqueue(rlt.LinkIndex);
        }


        #region Internal

        /// <summary>
        ///     Current RPCLinks.
        /// </summary>
        internal Dictionary<ushort, RpcLink> RpcLinks = new();

        /// <summary>
        ///     RPCLink indexes which can be used.
        /// </summary>
        private readonly Queue<ushort> _availableRpcLinkIndexes = new();

        #endregion
    }
}