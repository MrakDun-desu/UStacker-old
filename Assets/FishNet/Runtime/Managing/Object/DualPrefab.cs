using System;
using FishNet.Object;

namespace FishNet.Managing.Object
{
    /// <summary>
    ///     When using dual prefabs, defines which prefab to spawn for server, and which for clients.
    /// </summary>
    [Serializable]
    public struct DualPrefab
    {
        public NetworkObject Server;
        public NetworkObject Client;
    }
}