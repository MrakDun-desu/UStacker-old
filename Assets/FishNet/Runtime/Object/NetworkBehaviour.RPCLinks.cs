using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FishNet.Object.Helping;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

namespace FishNet.Object
{
    public abstract partial class NetworkBehaviour : MonoBehaviour
    {
        #region Private.

        /// <summary>
        ///     Link indexes for RPCs.
        /// </summary>
        private readonly Dictionary<uint, RpcLinkType> _rpcLinks = new();

        #endregion

        /// <summary>
        ///     Initializes RpcLinks. This will only call once even as host.
        /// </summary>
        private void InitializeOnceRpcLinks()
        {
            if (NetworkManager.IsServer)
            {
                /* Link only data from server to clients. While it is
                 * just as easy to link client to server it's usually
                 * not needed because server out data is more valuable
                 * than server in data. */
                /* Links will be stored in the NetworkBehaviour so that
                 * when the object is destroyed they can be added back
                 * into availableRpcLinks, within the ServerManager. */

                var serverManager = NetworkManager.ServerManager;
                //ObserverRpcs.
                foreach (var rpcHash in _observersRpcDelegates.Keys)
                    if (!MakeLink(rpcHash, RpcType.Observers))
                        return;
                //TargetRpcs.
                foreach (var rpcHash in _targetRpcDelegates.Keys)
                    if (!MakeLink(rpcHash, RpcType.Target))
                        return;
                //ReconcileRpcs.
                foreach (var rpcHash in _reconcileRpcDelegates.Keys)
                    if (!MakeLink(rpcHash, RpcType.Reconcile))
                        return;

                /* Tries to make a link and returns if
                 * successful. When a link cannot be made the method
                 * should exit as no other links will be possible. */
                bool MakeLink(uint rpcHash, RpcType rpcType)
                {
                    if (serverManager.GetRpcLink(out var linkIndex))
                    {
                        _rpcLinks[rpcHash] = new RpcLinkType(linkIndex, rpcType);
                        return true;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        ///     Returns an estimated length for any Rpc header.
        /// </summary>
        /// <returns></returns>
        private int GetEstimatedRpcHeaderLength()
        {
            /* Imaginary number for how long RPC headers are.
            * They are well under this value but this exist to
            * ensure a writer of appropriate length is pulled
            * from the pool. */
            return 20;
        }

        /// <summary>
        ///     Creates a PooledWriter and writes the header for a rpc.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private PooledWriter CreateLinkedRpc(RpcLinkType link, PooledWriter methodWriter, Channel channel)
        {
            var rpcHeaderBufferLength = GetEstimatedRpcHeaderLength();
            var methodWriterLength = methodWriter.Length;
            //Writer containing full packet.
            var writer = WriterPool.GetWriter(rpcHeaderBufferLength + methodWriterLength);
            writer.WriteUInt16(link.LinkIndex);
            //Write length only if reliable.
            if (channel == Channel.Reliable)
                writer.WriteLength(methodWriter.Length);
            //Data.
            writer.WriteArraySegment(methodWriter.GetArraySegment());

            return writer;
        }

        /// <summary>
        ///     Returns RpcLinks the ServerManager.
        /// </summary>
        private void ReturnRpcLinks()
        {
            if (_rpcLinks.Count == 0)
                return;

            ServerManager?.StoreRpcLinks(_rpcLinks);
            _rpcLinks.Clear();
        }

        /// <summary>
        ///     Writes rpcLinks to writer.
        /// </summary>
        internal void WriteRpcLinks(Writer writer)
        {
            var rpcLinkWriter = WriterPool.GetWriter();
            foreach (var item in _rpcLinks)
            {
                //RpcLink index.
                rpcLinkWriter.WriteUInt16(item.Value.LinkIndex);
                //Hash.
                rpcLinkWriter.WriteUInt16((ushort) item.Key);
                //True/false if observersRpc.
                rpcLinkWriter.WriteByte((byte) item.Value.RpcType);
            }

            writer.WriteBytesAndSize(rpcLinkWriter.GetBuffer(), 0, rpcLinkWriter.Length);
            rpcLinkWriter.Dispose();
        }
    }
}