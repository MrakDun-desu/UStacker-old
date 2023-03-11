using FishNet;
using FishNet.Connection;
using UnityEngine;
using UnityEngine.UI;
using UStacker.Common.Alerts;
using UStacker.GameSettings;
using UStacker.Multiplayer.Broadcasts;
using UStacker.Multiplayer.Settings;

namespace UStacker.Multiplayer.LobbyUi
{
    public class MultiplayerSettingsController : MonoBehaviour
    {
        [SerializeField] private GameObject _hostSettings;
        [SerializeField] private Button _saveSettingsButton;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private GameObject _playerSettings;
        [SerializeField] private GameSettingsSO _gameSettings;
        [SerializeField] private MultiplayerLobbySettingsSo _lobbySettings;

        private MultiplayerGameSettings _multiplayerGameSettings;
        private void Awake()
        {
            _multiplayerGameSettings = new MultiplayerGameSettings
            {
                GameSettings = _gameSettings.Settings,
                LobbySettings = _lobbySettings.Settings
            };
            
            _saveSettingsButton.onClick.AddListener(OnSaveButtonClicked);
            Player.LocalPlayerStarted += OnConnected;
            InstanceFinder.ServerManager.RegisterBroadcast<GameSettingsBroadcast>(OnSettingsBroadcastServer);
            InstanceFinder.ClientManager.RegisterBroadcast<SettingsReceivedBroadcast>(OnSettingsReceived);
        }

        private void OnSettingsBroadcastServer(NetworkConnection sender, GameSettingsBroadcast settingsBroadcast)
        {
            if (!Player.ConnectedPlayers[sender.ClientId].HasHostPrivileges)
                return;

            _multiplayerGameSettings = settingsBroadcast.GetSettings();
            _gameSettings.Settings = _multiplayerGameSettings.GameSettings;
            _lobbySettings.Settings = _multiplayerGameSettings.LobbySettings;
            
            InstanceFinder.ServerManager.Broadcast(sender, new SettingsReceivedBroadcast());
        }

        private void OnSettingsReceived(SettingsReceivedBroadcast broadcast)
        {
            AlertDisplayer.Instance.ShowAlert(new Alert("Settings saved!", "", AlertType.Success));
            _startGameButton.interactable = true;
        }

        private void OnDestroy()
        {
            Player.LocalPlayerStarted -= OnConnected;
        }

        private void OnConnected(Player localPlayer)
        {
            _hostSettings.SetActive(localPlayer.HasHostPrivileges);
            _saveSettingsButton.gameObject.SetActive(localPlayer.HasHostPrivileges);
            _playerSettings.SetActive(!localPlayer.HasHostPrivileges);
        }

        private void OnSaveButtonClicked()
        {
            _startGameButton.interactable = false;
            InstanceFinder.ClientManager.Broadcast(new GameSettingsBroadcast(_multiplayerGameSettings));
        }
    }
}