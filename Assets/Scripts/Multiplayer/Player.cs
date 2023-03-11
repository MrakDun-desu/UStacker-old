using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace UStacker.Multiplayer
{
    public class Player : NetworkBehaviour
    {
        public static readonly Dictionary<int, Player> ConnectedPlayers = new();
        public static event Action<int> PlayerAdded;
        public static event Action<int> PlayerRemoved;

        public static event Action<Player> LocalPlayerStarted;

        private static bool _clientStateRegistered;
        
        [field: SyncVar(OnChange = nameof(OnDisplayNameChange))]
        public string DisplayName { get; [ServerRpc] private set; }

        [field: SyncVar(OnChange = nameof(OnSpectateChange))]
        public bool IsSpectating { get; [ServerRpc] set; }
        
        [field: SyncVar(OnChange = nameof(OnGamesPlayedChange))]
        public uint GamesPlayed { get; [ServerRpc] set; }
        
        [field: SyncVar(OnChange = nameof(OnGamesWonChange))]
        public uint GamesWon { get; [ServerRpc] set; }

        // Temporary solution for host privileges until remote servers
        public bool HasHostPrivileges => IsHost;
        
        public event Action<string> DisplayNameChanged;
        public event Action<bool> SpectateChanged;
        public event Action<uint> GamesPlayedChanged;
        public event Action<uint> GamesWonChanged;
        public event Action Disconnected;

        public override void OnOwnershipClient(NetworkConnection prevOwner)
        {
            base.OnOwnershipClient(prevOwner);
            if (prevOwner.ClientId != -1)
            {
                ConnectedPlayers.Remove(prevOwner.ClientId);
                PlayerRemoved?.Invoke(prevOwner.ClientId);
            }

            ConnectedPlayers[OwnerId] = this;
            if (IsOwner)
            {
                DisplayName = $"Player {OwnerId + 1}";
                LocalPlayerStarted?.Invoke(this);
            }
            PlayerAdded?.Invoke(OwnerId);
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            PlayerRemoved?.Invoke(OwnerId);
            Disconnected?.Invoke();
            ConnectedPlayers.Remove(OwnerId);
        }

        private void OnDisplayNameChange(string _prevName, string newName, bool _asServer) => DisplayNameChanged?.Invoke(newName);

        private void OnSpectateChange(bool _prevVal, bool newVal, bool _asServer) => SpectateChanged?.Invoke(newVal);

        private void OnGamesWonChange(uint _prevVal, uint newVal, bool _asServer) => GamesWonChanged?.Invoke(newVal);
        
        private void OnGamesPlayedChange(uint _prevVal, uint newVal, bool _asServer) => GamesPlayedChanged?.Invoke(newVal);
    }
}