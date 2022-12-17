using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Blockstacker.Common;
using Blockstacker.GameSettings.SettingGroups;
using Newtonsoft.Json;
using UnityEngine;

namespace Blockstacker.GameSettings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Blockstacker/Game settings asset")]
    public class GameSettingsSO : ScriptableObject
    {
        private const string FILENAME_EXTENSION = ".json";
        private const char INVALID_CHAR_REPLACEMENT = '_';
        public StringReferenceSO GameType;
        public SettingsContainer Settings = new();
        public GeneralSettings General => Settings.General;
        public ControlsSettings Controls => Settings.Controls;
        public BoardDimensionsSettings BoardDimensions => Settings.BoardDimensions;
        public GravitySettings Gravity => Settings.Gravity;
        public ObjectiveSettings Objective => Settings.Objective;
        public PresentationSettings Presentation => Settings.Presentation;

        public event Action SettingsReloaded;

        public static IEnumerable<string> EnumeratePresets()
        {
            if (!Directory.Exists(PersistentPaths.GameSettingsPresets))
                Directory.CreateDirectory(PersistentPaths.GameSettingsPresets);

            foreach (var filename in Directory.EnumerateFiles(PersistentPaths.GameSettingsPresets)) yield return Path.GetFileNameWithoutExtension(filename);
        }

        public string Save(string presetName)
        {
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
                presetName = presetName.Replace(invalidChar, INVALID_CHAR_REPLACEMENT);

            if (!Directory.Exists(PersistentPaths.GameSettingsPresets))
                Directory.CreateDirectory(PersistentPaths.GameSettingsPresets);

            var savePath = Path.Combine(PersistentPaths.GameSettingsPresets, presetName);
            var actualSavePath = savePath;

            for (var i = 0; File.Exists(actualSavePath); i++)
            {
                actualSavePath = $"{savePath}_{i}";
                i++;
            }

            actualSavePath += FILENAME_EXTENSION;

            File.WriteAllText(actualSavePath,
                JsonConvert.SerializeObject(Settings, StaticSettings.DefaultSerializerSettings));
            return actualSavePath;
        }

        public bool TryLoad(string presetName)
        {
            var path = Path.Combine(PersistentPaths.GameSettingsPresets, presetName);
            path += FILENAME_EXTENSION;
            if (!File.Exists(path)) return false;

            Settings = JsonConvert.DeserializeObject<SettingsContainer>(File.ReadAllText(path),
                StaticSettings.DefaultSerializerSettings);

            SettingsReloaded?.Invoke();
            return true;
        }

        public void SetValue<T>(T value, string[] path)
        {
            if (path.Length == 0) return;
            PropertyInfo propertyInfo = null;
            object oldObject = null;
            object obj = Settings;
            var type = obj.GetType();
            foreach (var fieldName in path)
            {
                propertyInfo = type.GetProperty(fieldName);

                oldObject = obj;
                obj = propertyInfo?.GetValue(obj);

                if (obj is null) return;

                type = obj.GetType();
            }

            if (type != typeof(T)) return;

            propertyInfo?.SetValue(oldObject, value);
        }

        public T GetValue<T>(string[] path)
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

        public bool SettingExists<T>(string[] path)
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

        [Serializable]
        public record SettingsContainer
        {
            [field: SerializeField]
            public GeneralSettings General { get; set; } = new();
            [field: SerializeField]
            public ControlsSettings Controls { get; set; } = new();
            [field: SerializeField]
            public BoardDimensionsSettings BoardDimensions { get; set; } = new();
            [field: SerializeField]
            public GravitySettings Gravity { get; set; } = new();
            [field: SerializeField]
            public ObjectiveSettings Objective { get; set; } = new();
            [field: SerializeField]
            public PresentationSettings Presentation { get; set; } = new();
        }
    }
}