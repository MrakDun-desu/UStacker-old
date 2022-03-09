using Blockstacker.AppSettings.SettingGroups;

namespace Blockstacker.AppSettings
{
    public static class AppSettings
    {
        public static HandlingSettings Handling = new HandlingSettings();
        public static SoundSettings Sound = new SoundSettings();
        public static GameplaySettings Gameplay = new GameplaySettings();
        public static VideoSettings Video = new VideoSettings();
        public static CustomizationSettings Customization = new CustomizationSettings();
        public static OtherSettings Others = new OtherSettings();
    }
}