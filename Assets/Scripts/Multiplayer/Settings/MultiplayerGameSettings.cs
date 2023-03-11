using UStacker.GameSettings;

namespace UStacker.Multiplayer.Settings
{
    public class MultiplayerGameSettings
    {
        public MultiplayerLobbySettingsSo.SettingsContainer LobbySettings { get; set; }
        public GameSettingsSO.SettingsContainer GameSettings { get; set; }
    }
}