using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;
using UStacker.Common.Alerts;
using UStacker.GameSettings;
using UStacker.GameSettings.Enums;

namespace UStacker.Multiplayer.Settings
{
    public class MultiplayerSettingsManager : NetworkBehaviour
    {
        [SerializeField] private GameObject _hostSettings;
        [SerializeField] private Button _saveSettingsButton;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private GameObject _playerSettings;
        [SerializeField] private GameSettingsSO _gameSettings;
        [SerializeField] private MultiplayerLobbySettingsSo _lobbySettings;
        [SerializeField] private RotationSystemSO _srsRotationSystemSo;
        [SerializeField] private RotationSystemSO _srsPlusRotationSystemSo;

        [SerializeField] private MultiplayerGameSettings _settings;

        private MultiplayerGameSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                SettingsStatic = value;
                if (_settings is null)
                    return;

                _lobbySettings.Settings = _settings.LobbySettings;
                _gameSettings.Settings = _settings.GameSettings;

                if (IsServer)
                    LoadSettingsRotationSystem();
            }
        }

        // static for easy usage by other scripts
        public static MultiplayerGameSettings SettingsStatic { get; private set; }

        private void Awake()
        {
            _saveSettingsButton.onClick.AddListener(OnSaveButtonClicked);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            Settings = new MultiplayerGameSettings(_lobbySettings.Settings, _gameSettings.Settings);
        }

        private void OnDestroy()
        {
            Settings = null;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            var localPlayer = Player.LocalPlayer;
            _hostSettings.SetActive(localPlayer.HasHostPrivileges);
            _saveSettingsButton.gameObject.SetActive(localPlayer.HasHostPrivileges);
            _playerSettings.SetActive(!localPlayer.HasHostPrivileges);

            // if we're not the host, we need to get the current settings from the server
            if (!IsHost)
                RequestSettings();
        }

        [Client]
        private void OnSaveButtonClicked()
        {
            if (Player.LocalPlayer is null || !Player.LocalPlayer.HasHostPrivileges)
            {
                AlertDisplayer.ShowAlert(new Alert("Permission denied!", AlertType.Warning));
                return;
            }

            _startGameButton.interactable = false;
            SynchronizeSettings(Settings);
        }

        // ReSharper disable once UnusedParameter.Local
        // ReSharper disable once MemberCanBeMadeStatic.Local
        [TargetRpc]
        private void ConfirmSettingsSynchronized(NetworkConnection _target = null)
        {
            AlertDisplayer.ShowAlert(new Alert("Settings saved!", AlertType.Success));
            _startGameButton.interactable = true;
        }

        [Server]
        private void LoadSettingsRotationSystem()
        {
            _settings.GameSettings.Controls.ActiveRotationSystem =
                _settings.GameSettings.Controls.RotationSystemType switch
                {
                    RotationSystemType.SRS => _srsRotationSystemSo.RotationSystem,
                    RotationSystemType.SRSPlus => _srsPlusRotationSystemSo.RotationSystem,
                    RotationSystemType.None => new RotationSystem(),
                    RotationSystemType.Custom => _settings.GameSettings.Controls.ActiveRotationSystem,
                    _ => new RotationSystem()
                };
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestSettings(NetworkConnection sender = null)
        {
            SendSettingsToClients(sender, Settings);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SynchronizeSettings(MultiplayerGameSettings settings, NetworkConnection sender = null)
        {
            if (sender is null || !Player.ConnectedPlayers[sender.ClientId].HasHostPrivileges)
                return;

            Settings = settings;
            SendSettingsToClients(null, Settings);
            ConfirmSettingsSynchronized(sender);
        }

        [ObserversRpc]
        [TargetRpc]
        // ReSharper disable once UnusedParameter.Local
        private void SendSettingsToClients(NetworkConnection _target, MultiplayerGameSettings settings)
        {
            Settings = settings;
        }
    }
}