using FishNet.Broadcast;
using Newtonsoft.Json;
using UStacker.Common;
using UStacker.Multiplayer.Settings;

namespace UStacker.Multiplayer.Broadcasts
{
    public readonly struct GameSettingsBroadcast : IBroadcast
    {
        public readonly byte[] SettingsBytes;

        public GameSettingsBroadcast(MultiplayerGameSettings settings)
        {
            SettingsBytes =
                FileHandling.Zip(JsonConvert.SerializeObject(settings, StaticSettings.DefaultSerializerSettings));
        }

        public MultiplayerGameSettings GetSettings()
        {
            return JsonConvert.DeserializeObject<MultiplayerGameSettings>(FileHandling.Unzip(SettingsBytes),
                StaticSettings.DefaultSerializerSettings);
        }
    }
}