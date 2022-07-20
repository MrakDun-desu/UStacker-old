using System;
using System.IO;
using System.Reflection;
using Blockstacker.Common;
using Blockstacker.GlobalSettings.Groups;
using Newtonsoft.Json;

namespace Blockstacker.GlobalSettings
{
    public static class AppSettings
    {
        private static SettingsContainer Settings = new();
        public static HandlingSettings Handling => Settings.Handling;
        public static SoundSettings Sound => Settings.Sound;
        public static GameplaySettings Gameplay => Settings.Gameplay;
        public static VideoSettings Video => Settings.Video;
        public static CustomizationSettings Customization => Settings.Customization;
        public static OtherSettings Other => Settings.Others;

        public static event Action SettingsReloaded;

        public static string Rebinds
        {
            get => Settings.Rebinds;
            set => Settings.Rebinds = value;
        }


        public static bool TrySave(string path = null)
        {
            path ??= CustomizationPaths.GlobalSettings;
            
            if (!Directory.Exists(Path.GetDirectoryName(path))) return false;
            File.WriteAllText(path, JsonConvert.SerializeObject(Settings, StaticSettings.JsonSerializerSettings));
            return true;
        }

        public static bool TryLoad(string path = null)
        {
            path ??= CustomizationPaths.GlobalSettings;
            if (!File.Exists(path))
                return false;

            Settings = JsonConvert.DeserializeObject<SettingsContainer>(File.ReadAllText(path), StaticSettings.JsonSerializerSettings);
            Settings ??= new SettingsContainer();
            SettingsReloaded?.Invoke();
            return true;
        }

        public static bool TrySetValue<T>(T value, string[] path)
        {
            if (path.Length == 0) return false;
            FieldInfo fieldInfo = null;
            object oldObject = null;
            object obj = Settings;
            var type = obj.GetType();
            foreach (var fieldName in path)
            {
                fieldInfo = type.GetField(fieldName);
                if (fieldInfo == null) return false;

                oldObject = obj;
                obj = fieldInfo.GetValue(obj);
                if (obj == null) return false;

                type = obj.GetType();
            }

            if (type != typeof(T)) return false;

            fieldInfo?.SetValue(oldObject, value);
            return true;
        }

        public static T GetValue<T>(string[] path)
        {
            if (path.Length == 0) return default;
            object obj = Settings;
            var type = obj.GetType();
            foreach (var fieldName in path)
            {
                var fieldInfo = type.GetField(fieldName);
                if (fieldInfo == null) return default;

                obj = fieldInfo.GetValue(obj);
                if (obj == null) return default;

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
                var fieldInfo = type.GetField(fieldName);
                if (fieldInfo is null) return false;

                obj = fieldInfo.GetValue(obj);
                if (obj is null) return false;

                type = obj.GetType();
            }

            return type == typeof(T);
        }

        [Serializable]
        internal record SettingsContainer
        {
            public HandlingSettings Handling = new();
            public SoundSettings Sound = new();
            public GameplaySettings Gameplay = new();
            public VideoSettings Video = new();
            public CustomizationSettings Customization = new();
            public OtherSettings Others = new();
            public string Rebinds;
        }
    }
}