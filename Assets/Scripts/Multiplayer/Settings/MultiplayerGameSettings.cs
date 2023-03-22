using System;
using FishNet.Serializing;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UStacker.Common;
using UStacker.GameSettings;

namespace UStacker.Multiplayer.Settings
{
    [UsedImplicitly]
    public static class MultiplayerGameSettingsSerializer
    {
        [UsedImplicitly]
        public static void WriteSettings(this Writer writer, MultiplayerGameSettings settings)
        {
            var settingsBytes =
                FileHandling.Zip(JsonConvert.SerializeObject(settings, StaticSettings.DefaultSerializerSettings));
            
            writer.WriteBytesAndSize(settingsBytes);
        }

        [UsedImplicitly]
        public static MultiplayerGameSettings ReadSettings(this Reader reader)
        {
            var settingsBytes = reader.ReadBytesAndSizeAllocated();
            return JsonConvert.DeserializeObject<MultiplayerGameSettings>(FileHandling.Unzip(settingsBytes),
                StaticSettings.DefaultSerializerSettings);
        }
    }
    
    [Serializable]
    public class MultiplayerGameSettings
    {
        [SerializeField]
        private MultiplayerLobbySettingsSo.SettingsContainer _lobbySettings;
        [SerializeField]
        private GameSettingsSO.SettingsContainer _gameSettings;

        public MultiplayerLobbySettingsSo.SettingsContainer LobbySettings => _lobbySettings;
        public GameSettingsSO.SettingsContainer GameSettings => _gameSettings;

        public MultiplayerGameSettings(MultiplayerLobbySettingsSo.SettingsContainer lobbySettings,
            GameSettingsSO.SettingsContainer gameSettings)
        {
            _lobbySettings = lobbySettings;
            _gameSettings = gameSettings;
        }
    }
}