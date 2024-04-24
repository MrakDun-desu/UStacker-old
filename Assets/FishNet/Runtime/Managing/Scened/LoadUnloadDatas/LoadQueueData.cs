using System;
using System.Runtime.CompilerServices;
using FishNet.Connection;
using FishNet.Utility.Constant;

[assembly: InternalsVisibleTo(UtilityConstants.GENERATED_ASSEMBLY_NAME)]

namespace FishNet.Managing.Scened
{
    /// <summary>
    ///     Data generated when loading a scene.
    /// </summary>
    public class LoadQueueData
    {
        /// <summary>
        ///     True if to iterate this queue data as server.
        /// </summary>
        [NonSerialized] public readonly bool AsServer;

        /// <summary>
        ///     Connections to load scenes for. Only valid on the server and when ScopeType is Connections.
        /// </summary>
        [NonSerialized] public NetworkConnection[] Connections = new NetworkConnection[0];

        /// <summary>
        ///     Current global scenes.
        /// </summary>
        public string[] GlobalScenes = new string[0];

        /// <summary>
        ///     SceneLoadData to use.
        /// </summary>
        public SceneLoadData SceneLoadData;

        /// <summary>
        ///     Clients which receive this SceneQueueData. If Networked, all clients do. If Connections, only the specified
        ///     Connections do.
        /// </summary>
        [NonSerialized] public SceneScopeType ScopeType;

        public LoadQueueData()
        {
        }

        internal LoadQueueData(SceneScopeType scopeType, NetworkConnection[] conns, SceneLoadData sceneLoadData,
            string[] globalScenes, bool asServer)
        {
            ScopeType = scopeType;
            Connections = conns;
            SceneLoadData = sceneLoadData;
            GlobalScenes = globalScenes;
            AsServer = asServer;
        }
    }
}