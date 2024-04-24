using System;
using System.Runtime.CompilerServices;
using FishNet.Connection;
using FishNet.Utility.Constant;

[assembly: InternalsVisibleTo(UtilityConstants.GENERATED_ASSEMBLY_NAME)]

namespace FishNet.Managing.Scened
{
    /// <summary>
    ///     Data generated when unloading a scene.
    /// </summary>
    public class UnloadQueueData
    {
        /// <summary>
        ///     True if to iterate this queue data as server.
        /// </summary>
        [NonSerialized] public readonly bool AsServer;

        /// <summary>
        ///     Clients which receive this SceneQueueData. If Networked, all clients do. If Connections, only the specified
        ///     Connections do.
        /// </summary>
        [NonSerialized] public readonly SceneScopeType ScopeType;

        /// <summary>
        ///     Connections to unload scenes for. Only valid on the server and when ScopeType is Connections.
        /// </summary>
        [NonSerialized] public NetworkConnection[] Connections;

        /// <summary>
        ///     Current global scenes.
        /// </summary>
        public string[] GlobalScenes = new string[0];

        /// <summary>
        ///     SceneUnloadData to use.
        /// </summary>
        public SceneUnloadData SceneUnloadData;

        public UnloadQueueData()
        {
        }

        internal UnloadQueueData(SceneScopeType scopeType, NetworkConnection[] conns, SceneUnloadData sceneUnloadData,
            string[] globalScenes, bool asServer)
        {
            ScopeType = scopeType;
            Connections = conns;
            SceneUnloadData = sceneUnloadData;
            GlobalScenes = globalScenes;
            AsServer = asServer;
        }
    }
}