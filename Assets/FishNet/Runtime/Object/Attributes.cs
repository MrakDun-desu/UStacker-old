using System;
using FishNet.Managing.Logging;
using FishNet.Transporting;
using UnityEngine;

namespace FishNet.Object
{
    /// <summary>
    ///     ServerRpc methods will send messages to the server.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ServerRpcAttribute : Attribute
    {
        /// <summary>
        ///     Estimated length of data being sent.
        ///     When a value other than -1 the minimum length of the used serializer will be this value.
        ///     This is useful for writing large packets which otherwise resize the serializer.
        /// </summary>
        public int DataLength = -1;

        /// <summary>
        ///     True to only allow the owning client to call this RPC.
        /// </summary>
        public bool RequireOwnership = true;

        /// <summary>
        ///     True to also run the RPC logic locally.
        /// </summary>
        public bool RunLocally = false;
    }

    /// <summary>
    ///     ObserversRpc methods will send messages to all observers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ObserversRpcAttribute : Attribute
    {
        /// <summary>
        ///     True to buffer the last value and send it to new players when the object is spawned for them.
        ///     RPC will be sent on the same channel as the original RPC, and immediately before the OnSpawnServer override.
        /// </summary>
        public bool BufferLast = false;

        /// <summary>
        ///     Estimated length of data being sent.
        ///     When a value other than -1 the minimum length of the used serializer will be this value.
        ///     This is useful for writing large packets which otherwise resize the serializer.
        /// </summary>
        public int DataLength = -1;

        /// <summary>
        ///     True to exclude the owner from receiving this RPC.
        /// </summary>
        public bool ExcludeOwner = false;

        /// <summary>
        ///     True to prevent the connection from receiving this Rpc if they are also server.
        /// </summary>
        public bool ExcludeServer = false;

        /// <summary>
        ///     True to also run the RPC logic locally.
        /// </summary>
        public bool RunLocally = false;
    }

    /// <summary>
    ///     TargetRpc methods will send messages to a single client.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TargetRpcAttribute : Attribute
    {
        /// <summary>
        ///     Estimated length of data being sent.
        ///     When a value other than -1 the minimum length of the used serializer will be this value.
        ///     This is useful for writing large packets which otherwise resize the serializer.
        /// </summary>
        public int DataLength = -1;

        /// <summary>
        ///     True to prevent the connection from receiving this Rpc if they are also server.
        /// </summary>
        public bool ExcludeServer = false;

        /// <summary>
        ///     True to also run the RPC logic locally.
        /// </summary>
        public bool RunLocally = false;

        /// <summary>
        ///     True to validate the target is possible and output debug when not.
        ///     Use this field with caution as it may create undesired results when set to false.
        /// </summary>
        public bool ValidateTarget = true;
    }

    /// <summary>
    ///     Prevents a method from running if server is not active.
    ///     <para>Can only be used inside a NetworkBehaviour</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ServerAttribute : Attribute
    {
        /// <summary>
        ///     Type of logging to use when the IsServer check fails.
        /// </summary>
        public LoggingType Logging = LoggingType.Warning;
    }

    /// <summary>
    ///     Prevents this method from running if client is not active.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ClientAttribute : Attribute
    {
        /// <summary>
        ///     Type of logging to use when the IsClient check fails.
        /// </summary>
        public LoggingType Logging = LoggingType.Warning;

        /// <summary>
        ///     True to only allow a client to run the method if they are owner of the object.
        /// </summary>
        public bool RequireOwnership = false;
    }
}


namespace FishNet.Object.Synchronizing
{
    /// <summary>
    ///     Synchronizes collections or objects from the server to clients. Can be used with custom SyncObjects.
    ///     Value must be changed on server.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SyncObjectAttribute : PropertyAttribute
    {
        /// <summary>
        ///     Clients which may receive value updates.
        /// </summary>
        public ReadPermission ReadPermissions = ReadPermission.Observers;

        /// <summary>
        ///     True if to require the readonly attribute.
        ///     Setting to false will allow inspector serialization of this object, but you must never manually initialize this
        ///     object.
        /// </summary>
        public bool RequireReadOnly = true;

        /// <summary>
        ///     How often values may update over the network.
        /// </summary>
        public float SendRate = 0.1f;
    }

    /// <summary>
    ///     Synchronizes a variable from server to clients automatically.
    ///     Value must be changed on server.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SyncVarAttribute : PropertyAttribute
    {
        /// <summary>
        ///     Channel to use. Unreliable SyncVars will use eventual consistency.
        /// </summary>
        public Channel Channel;

        /// <summary>
        ///     Method which will be called on the server and clients when the value changes.
        /// </summary>
        public string OnChange;

        /// <summary>
        ///     Clients which may receive value updates.
        /// </summary>
        public ReadPermission ReadPermissions = ReadPermission.Observers;

        /// <summary>
        ///     How often values may update over the network.
        /// </summary>
        public float SendRate = 0.1f;
    }
}