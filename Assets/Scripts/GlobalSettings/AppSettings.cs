using System;
using System.IO;
using System.Reflection;
using Blockstacker.GlobalSettings.Groups;
using UnityEngine;

namespace Blockstacker.GlobalSettings
{
    public static class AppSettings
    {
        private static readonly SettingsContainer Settings = new();
        public static HandlingSettings Handling => Settings.Handling;
        public static SoundSettings Sound => Settings.Sound;
        public static GameplaySettings Gameplay => Settings.Gameplay;
        public static VideoSettings Video => Settings.Video;
        public static CustomizationSettings Customization => Settings.Customization;
        public static OtherSettings Other => Settings.Others;

        public static string Rebinds
        {
            get => Settings.Rebinds;
            set => Settings.Rebinds = value;
        }

        private static string SettingsPath => Path.Combine(Application.persistentDataPath, "appSettings.json");

        public static bool TrySave(string path = null)
        {
            path ??= SettingsPath;
            var slashIndex = path.LastIndexOfAny(new[] {'/', '\\'});
            if (!Directory.Exists(path[..slashIndex]) || string.IsNullOrEmpty(path[..slashIndex])) return false;
            File.WriteAllText(path, JsonUtility.ToJson(Settings, true));
            return true;
        }

        public static bool TryLoad(string path = null)
        {
            path ??= SettingsPath;
            if (!File.Exists(path))
                return false;

            JsonUtility.FromJsonOverwrite(File.ReadAllText(path), Settings);
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