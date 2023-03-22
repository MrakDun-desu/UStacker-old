using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UStacker.Common.Alerts;
using UStacker.GlobalSettings;
using UStacker.GlobalSettings.Groups;
using UStacker.Multiplayer.Settings;

namespace UStacker.Multiplayer
{
    public class Player : NetworkBehaviour
    {
        public static readonly Dictionary<int, Player> ConnectedPlayers = new();

        public static IEnumerable<Player> ActivePlayers =>
            ConnectedPlayers.Values.Where(player => !player.IsSpectating);

        public static event Action<int> PlayerJoined;
        public static event Action<int> PlayerLeft;
        public static event Action<Player> LocalPlayerStarted;

        public static Player LocalPlayer;

        [field: SyncVar(OnChange = nameof(OnDisplayNameChange))]
        public string DisplayName { get; [ServerRpc] private set; }

        [SyncVar(OnChange = nameof(OnSpectateChange))]
        private bool _isSpectating;

        public bool IsSpectating
        {
            get => _isSpectating;
            [ServerRpc]
            set
            {
                if (MultiplayerSettingsManager.SettingsStatic is null) return;
                if (!value && ActivePlayers.Count() >= MultiplayerSettingsManager.SettingsStatic.LobbySettings.PlayerLimit)
                {
                    DisplayRoomFullWarning(Owner);
                    return;
                }

                _isSpectating = value;
            }
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        [field: SyncVar(OnChange = nameof(OnGamesPlayedChange))]
        public uint GamesPlayed { get; [Server] set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        [field: SyncVar(OnChange = nameof(OnGamesWonChange))]
        public uint GamesWon { get; [Server] set; }

        private Coroutine _handlingChangeCor;
        public HandlingSettings Handling = new();
        
        public event Action<string> DisplayNameChanged;
        public event Action<bool> SpectateChanged;
        public event Action<uint> GamesWonChanged;
        public event Action<uint> GamesPlayedChanged;
        public event Action Disconnected;
        
        // Temporary solution for host privileges until remote servers
        public bool HasHostPrivileges => IsHost;
        

        public override void OnOwnershipClient(NetworkConnection prevOwner)
        {
            base.OnOwnershipClient(prevOwner);
            if (prevOwner is not null && prevOwner.ClientId != -1)
            {
                ConnectedPlayers.Remove(prevOwner.ClientId);
                PlayerLeft?.Invoke(prevOwner.ClientId);
            }

            ConnectedPlayers[OwnerId] = this;
            PlayerJoined?.Invoke(OwnerId);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            if (!IsOwner) return;
            
            DisplayName = $"Player {OwnerId + 1}";
            LocalPlayer = this;
            LocalPlayerStarted?.Invoke(this);
            Handling = AppSettings.Handling;
            Handling.Dirtied += OnHandlingChanged;
            SetHandlingServer(Handling);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            Handling.Dirtied -= OnHandlingChanged;
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            Disconnected?.Invoke();
            PlayerLeft?.Invoke(OwnerId);
            ConnectedPlayers.Remove(OwnerId);
            if (LocalPlayer == this)
                LocalPlayer = null;
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void SetHandlingClient(HandlingSettings newHandling)
        {
            Handling.Override(newHandling);
        }

        [ServerRpc]
        private void SetHandlingServer(HandlingSettings newHandling)
        {
            Handling.Override(newHandling);
            SetHandlingClient(Handling);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            if (MultiplayerSettingsManager.SettingsStatic is null)
                return;

            _isSpectating = ActivePlayers.Count() >= MultiplayerSettingsManager.SettingsStatic.LobbySettings.PlayerLimit;
        }
        
        // disabled unused parameters because FishNet needs to have all of them for OnChange methods
        // ReSharper disable UnusedParameter.Local
        private void OnDisplayNameChange(string _prevName, string newName, bool _asServer) =>
            DisplayNameChanged?.Invoke(newName);

        private void OnSpectateChange(bool _prevVal, bool newVal, bool _asServer) => SpectateChanged?.Invoke(newVal);

        private void OnGamesWonChange(uint _prevVal, uint newVal, bool _asServer) => GamesWonChanged?.Invoke(newVal);

        private void OnGamesPlayedChange(uint _prevVal, uint newVal, bool _asServer) =>
            GamesPlayedChanged?.Invoke(newVal);

        [TargetRpc]
        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void DisplayRoomFullWarning(NetworkConnection _)
            => AlertDisplayer.Instance.ShowAlert(new Alert("Room is full!", string.Empty, AlertType.Warning));

        // ReSharper restore UnusedParameter.Local
        
        private void OnHandlingChanged()
        {
            if (_handlingChangeCor is not null)
                return;

            _handlingChangeCor = StartCoroutine(RefreshHandlingCor());
        }
        
        private IEnumerator RefreshHandlingCor()
        {
            yield return new WaitForEndOfFrame();
            SetHandlingServer(Handling);
            Handling.Undirty();
            _handlingChangeCor = null;
        }
    }
}