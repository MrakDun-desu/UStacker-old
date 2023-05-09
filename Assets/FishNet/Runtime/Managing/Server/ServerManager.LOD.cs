using System.Runtime.CompilerServices;
using FishNet.Connection;
using FishNet.Serializing;
using UnityEngine;

namespace FishNet.Managing.Server
{
    public sealed partial class ServerManager : MonoBehaviour
    {
        /// <summary>
        ///     Parses a received network LOD update.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ParseNetworkLODUpdate(PooledReader reader, NetworkConnection conn)
        {
            if (!conn.Authenticated)
                return;

            Debug.ClearDeveloperConsole();

            //Get server objects to save calls.
            var serverObjects = Objects.Spawned;
            //Get level of details for this connection and reset them.
            var levelOfDetails = conn.LevelOfDetails;
            levelOfDetails.Clear();

            //Number of entries.
            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var objectId = reader.ReadNetworkObjectId();
                var lod = reader.ReadByte();
                if (serverObjects.TryGetValue(objectId, out var nob)) levelOfDetails[nob] = lod;
                // Debug.Log($"Level {lod}, Object {nob.name}");
            }
        }
    }
}