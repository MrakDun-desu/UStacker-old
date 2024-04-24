
/************************************
AppSettings.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UStacker.Common;
using UStacker.GlobalSettings.Groups;

namespace UStacker.GlobalSettings
{
    public static class AppSettings
    {
        private static SettingsContainer Settings = new();
        public static HandlingSettings Handling => Settings.Handling;
        public static SoundSettings Sound => Settings.Sound;
        public static GameplaySettings Gameplay => Settings.Gameplay;
        public static VideoSettings Video => Settings.Video;
        public static CustomizationSettings Customization => Settings.Customization;
        public static StatCountingSettings StatCounting => Settings.StatCounting;
        public static OtherSettings Other => Settings.Others;
        public static OverridesDictionary GameOverrides => Settings.GameOverrrides;

        public static string Rebinds
        {
            get => Settings.Rebinds;
            set => Settings.Rebinds = value;
        }

        public static event Action SettingsReloaded;

        public static async Task<bool> TrySaveAsync(string path = null)
        {
            const char INVALID_CHAR_REPLACEMENT = '_';
            if (path is not null)
            {
                var filename = Path.GetFileName(path);
                foreach (var invalidChar in Path.GetInvalidFileNameChars())
                    filename = filename.Replace(invalidChar, INVALID_CHAR_REPLACEMENT);

                path = Path.Combine(Path.GetDirectoryName(path) ?? string.Empty, filename);
            }

            path ??= PersistentPaths.GlobalSettings;

            if (!Directory.Exists(Path.GetDirectoryName(path))) return false;
            await File.WriteAllTextAsync(path,
                JsonConvert.SerializeObject(Settings, StaticSettings.DefaultSerializerSettings));
            return true;
        }

        public static async Task<bool> TryLoadAsync(string path = null)
        {
            path ??= PersistentPaths.GlobalSettings;
            if (!File.Exists(path))
                return false;

            Settings = JsonConvert.DeserializeObject<SettingsContainer>(await File.ReadAllTextAsync(path),
                StaticSettings.DefaultSerializerSettings);
            Settings ??= new SettingsContainer();

            SettingsReloaded?.Invoke();
            return true;
        }

        public static bool TrySetValue<T>(T value, string[] path)
        {
            if (path.Length == 0) return false;
            PropertyInfo propertyInfo = null;
            object oldObject = null;
            object obj = Settings;
            var type = obj.GetType();
            foreach (var fieldName in path)
            {
                propertyInfo = type.GetProperty(fieldName);

                if (propertyInfo is null) return false;

                oldObject = obj;
                obj = propertyInfo.GetValue(obj);

                if (obj is null) return false;

                type = obj.GetType();
            }

            if (type != typeof(T)) return false;

            if (propertyInfo is null) return false;

            propertyInfo.SetValue(oldObject, value);
            return true;
        }

        public static T GetValue<T>(string[] path)
        {
            if (path.Length == 0) return default;
            object obj = Settings;
            var type = obj.GetType();
            foreach (var fieldName in path)
            {
                var propertyInfo = type.GetProperty(fieldName);

                obj = propertyInfo?.GetValue(obj);

                if (obj is null) return default;

                type = obj.GetType();
            }

            if (type == typeof(T)) return (T) obj;
            return default;
        }

        public static bool SettingExists<T>(string[] path)
        {
            if (path.Length == 0) return false;
            object obj = Settings;
            var type = obj.GetType();
            foreach (var fieldName in path)
            {
                var propertyInfo = type.GetProperty(fieldName);

                obj = propertyInfo?.GetValue(obj);

                if (obj is null) return false;

                type = obj.GetType();
            }

            return type == typeof(T);
        }

        // for later usage for tooltips
        public static bool TryGetSettingAttribute<T>(string[] path, out T output) where T : Attribute
        {
            output = default;
            if (path.Length == 0) return false;
            object obj = Settings;
            var type = obj.GetType();
            PropertyInfo propertyInfo = null;
            foreach (var fieldName in path)
            {
                propertyInfo = type.GetProperty(fieldName);

                obj = propertyInfo?.GetValue(obj);
                if (obj is null) return false;

                type = obj.GetType();
            }

            output = propertyInfo?.GetCustomAttribute<T>(false);
            return output is not null;
        }

        internal record SettingsContainer
        {
            public OverridesDictionary GameOverrrides { get; } = new();

            // ReSharper disable MemberHidesStaticFromOuterClass
            public HandlingSettings Handling { get; } = new();
            public SoundSettings Sound { get; } = new();
            public GameplaySettings Gameplay { get; } = new();
            public VideoSettings Video { get; } = new();
            public CustomizationSettings Customization { get; } = new();
            public StatCountingSettings StatCounting { get; } = new();
            public OtherSettings Others { get; } = new();

            public string Rebinds { get; set; } = string.Empty;
            // ReSharper restore MemberHidesStaticFromOuterClass
        }
    }
}
/************************************
end AppSettings.cs
*************************************/
