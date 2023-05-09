
/************************************
LobbySettingsChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
using TMPro;
using UnityEngine;

namespace UStacker.Multiplayer.Settings
{
    public class LobbySettingsChanger : MonoBehaviour
    {
        [SerializeField] private MultiplayerLobbySettingsSo _lobbySettings;
        [SerializeField] private TMP_InputField _firstToField;
        [SerializeField] private TMP_InputField _winByField;
        [SerializeField] private TMP_InputField _playerLimitField;
        [SerializeField] private TMP_InputField _minimumPlayersField;

        private void Awake()
        {
            _firstToField.onEndEdit.AddListener(OnFirstToChanged);
            _winByField.onEndEdit.AddListener(OnWinByChanged);
            _playerLimitField.onEndEdit.AddListener(OnPlayerLimitChanged);
            _minimumPlayersField.onEndEdit.AddListener(OnMinimumPlayersChanged);

            _lobbySettings.SettingsReloaded += RefreshValue;

            RefreshValue();
        }

        private void OnDestroy()
        {
            _lobbySettings.SettingsReloaded -= RefreshValue;
        }

        private void RefreshValue()
        {
            _winByField.text = _lobbySettings.Settings.WinBy.ToString();
            _firstToField.text = _lobbySettings.Settings.FirstTo.ToString();
            _playerLimitField.text = _lobbySettings.Settings.PlayerLimit.ToString();
            _minimumPlayersField.text = _lobbySettings.Settings.MinimumPlayers.ToString();
        }

        private void OnWinByChanged(string value)
        {
            if (!uint.TryParse(value, out var uintValue))
            {
                _winByField.text = _lobbySettings.Settings.WinBy.ToString();
                return;
            }

            _lobbySettings.Settings.WinBy = uintValue;
            _winByField.text = _lobbySettings.Settings.WinBy.ToString();
        }

        private void OnFirstToChanged(string value)
        {
            if (!uint.TryParse(value, out var uintValue))
            {
                _firstToField.text = _lobbySettings.Settings.FirstTo.ToString();
                return;
            }

            _lobbySettings.Settings.FirstTo = uintValue;
            _firstToField.text = _lobbySettings.Settings.FirstTo.ToString();
        }

        private void OnMinimumPlayersChanged(string value)
        {
            if (!uint.TryParse(value, out var uintValue))
            {
                _minimumPlayersField.text = _lobbySettings.Settings.MinimumPlayers.ToString();
                return;
            }

            _lobbySettings.Settings.MinimumPlayers = uintValue;
            _minimumPlayersField.text = _lobbySettings.Settings.MinimumPlayers.ToString();
        }

        private void OnPlayerLimitChanged(string value)
        {
            if (!uint.TryParse(value, out var uintValue))
            {
                _playerLimitField.text = _lobbySettings.Settings.PlayerLimit.ToString();
                return;
            }

            _lobbySettings.Settings.PlayerLimit = uintValue;
            _playerLimitField.text = _lobbySettings.Settings.PlayerLimit.ToString();
        }
    }
}
/************************************
end LobbySettingsChanger.cs
*************************************/
