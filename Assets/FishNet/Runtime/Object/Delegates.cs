using System.Runtime.CompilerServices;
using FishNet.Connection;
using FishNet.Serializing;
using FishNet.Transporting;
using FishNet.Utility.Constant;

[assembly: InternalsVisibleTo(UtilityConstants.CODEGEN_ASSEMBLY_NAME)]

namespace FishNet.Object.Delegating
{
    public delegate void ServerRpcDelegate(PooledReader reader, Channel channel, NetworkConnection sender);

    public delegate void ClientRpcDelegate(PooledReader reader, Channel channel);
}