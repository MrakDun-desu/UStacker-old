using System;
using System.IO;
using System.Reflection;
using UStacker.Common;
using UStacker.GlobalSettings.Groups;
using Newtonsoft.Json;
using UnityEngine;

namespace UStacker.GlobalSettings
{
    public static class AppSettings
    {
        private const char INVALID_CHAR_REPLACEMENT = '_';
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

        public static bool TrySave(string path = null)
        {
            if (path is not null)
            {
                foreach (var invalidChar in Path.GetInvalidFileNameChars())
                    path = path.Replace(invalidChar, INVALID_CHAR_REPLACEMENT);
            }

            path ??= PersistentPaths.GlobalSettings;

            if (!Directory.Exists(Path.GetDirectoryName(path))) return false;
            File.WriteAllText(path, JsonConvert.SerializeObject(Settings, StaticSettings.DefaultSerializerSettings));
            return true;
        }

        public static bool TryLoad(string path = null)
        {
            path ??= PersistentPaths.GlobalSettings;
            if (!File.Exists(path))
                return false;

            Settings = JsonConvert.DeserializeObject<SettingsContainer>(File.ReadAllText(path),
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

        [Serializable]
        internal record SettingsContainer
        {
            [field: SerializeField] public HandlingSettings Handling { get; set; } = new();
            [field: SerializeField] public SoundSettings Sound { get; set; } = new();
            [field: SerializeField] public GameplaySettings Gameplay { get; set; } = new();
            [field: SerializeField] public VideoSettings Video { get; set; } = new();
            [field: SerializeField] public CustomizationSettings Customization { get; set; } = new();
            [field: SerializeField] public StatCountingSettings StatCounting { get; set; } = new();
            [field: SerializeField] public OtherSettings Others { get; set; } = new();
            [field: SerializeField] public string Rebinds { get; set; } = string.Empty;
            [field: SerializeField] public OverridesDictionary GameOverrrides { get; set; } = new();
        }
    }
}