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
        
        [SerializeField]
        private MultiplayerGameSettings _settings;

        private MultiplayerGameSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                SettingsStatic = value;
                if (_settings is null)
                    return;
                    
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
        }

        // static for easy usage by other scripts
        public static MultiplayerGameSettings SettingsStatic { get; private set; }

        public static bool SettingsSynchronized { get; private set; }

        private Player _localPlayer;

        private void Awake()
        {
            _saveSettingsButton.onClick.AddListener(OnSaveButtonClicked);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            Settings = new MultiplayerGameSettings(_lobbySettings.Settings, _gameSettings.Settings);
            SettingsSynchronized = true;
        }

        private void OnDestroy()
        {
            Settings = null;
            SettingsSynchronized = false;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            _localPlayer = Player.LocalPlayer;
            _hostSettings.SetActive(_localPlayer.HasHostPrivileges);
            _saveSettingsButton.gameObject.SetActive(_localPlayer.HasHostPrivileges);
            _playerSettings.SetActive(!_localPlayer.HasHostPrivileges);
            
            // if we're not the host, we need to get the current settings from the server
            if (!IsHost)
                RequestSettings();
        }

        [Client]
        private void OnSaveButtonClicked()
        {
            if (_localPlayer is null || !_localPlayer.HasHostPrivileges)
            {
                AlertDisplayer.ShowAlert(new Alert("Permission denied!", AlertType.Warning));
                return;
            }
            _startGameButton.interactable = false;
            SettingsSynchronized = false;
            SynchronizeSettings(Settings);
            AlertDisplayer.ShowAlert(new Alert("Settings saved!", AlertType.Success));
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestSettings(NetworkConnection sender = null)
        {
            SettingsSynchronized = false;
            if (Settings.GameSettings.Controls.RotationSystemType != RotationSystemType.Custom)
                Settings.GameSettings.Controls.ActiveRotationSystem = null;
            SendSettingsToClients(sender, Settings);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SynchronizeSettings(MultiplayerGameSettings settings, NetworkConnection sender = null)
        {
            if (sender is null || !Player.ConnectedPlayers[sender.ClientId].HasHostPrivileges)
                return;

            Settings = settings;
            
            Settings.GameSettings.Controls.ActiveRotationSystem =
            Settings.GameSettings.Controls.RotationSystemType switch
            {
                RotationSystemType.SRS => _srsRotationSystemSo.RotationSystem,
                RotationSystemType.SRSPlus => _srsPlusRotationSystemSo.RotationSystem,
                RotationSystemType.None => new RotationSystem(),
                RotationSystemType.Custom => Settings.GameSettings.Controls.ActiveRotationSystem,
                _ => new RotationSystem()
            };
            _lobbySettings.Settings = Settings.LobbySettings;
            _gameSettings.Settings = Settings.GameSettings;
            SendSettingsToClients(null, Settings);
            SettingsSynchronized = true;
        }

        [ObserversRpc][TargetRpc]
        // ReSharper disable once UnusedParameter.Local
        private void SendSettingsToClients(NetworkConnection _target, MultiplayerGameSettings settings)
        {
            Settings = settings;
            _lobbySettings.Settings = Settings.LobbySettings;
            _gameSettings.Settings = Settings.GameSettings;
        }
    }
}