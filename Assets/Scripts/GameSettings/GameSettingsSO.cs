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
        public StringReferenceSO GameType;
        public SettingsContainer Settings = new();
        public GeneralSettings General => Settings.General;
        public ControlsSettings Controls => Settings.Controls;
        public BoardDimensionsSettings BoardDimensions => Settings.BoardDimensions;
        public GravitySettings Gravity => Settings.Gravity;
        public ObjectiveSettings Objective => Settings.Objective;
        public PresentationSettings Presentation => Settings.Presentation;

        public event Action SettingsReloaded;
        private const string FILENAME_EXTENSION = ".json";
        private const char INVALID_CHAR_REPLACEMENT = '_';

        public static IEnumerable<string> EnumeratePresets()
        {
            if (!Directory.Exists(CustomizationPaths.GameSettingsPresets))
                Directory.CreateDirectory(CustomizationPaths.GameSettingsPresets);

            foreach (var filename in Directory.EnumerateFiles(CustomizationPaths.GameSettingsPresets))
            {
                yield return Path.GetFileNameWithoutExtension(filename);
            }
        }

        public string Save(string presetName)
        {
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
                presetName = presetName.Replace(invalidChar, INVALID_CHAR_REPLACEMENT);

            if (!Directory.Exists(CustomizationPaths.GameSettingsPresets))
                Directory.CreateDirectory(CustomizationPaths.GameSettingsPresets);

            var savePath = Path.Combine(CustomizationPaths.GameSettingsPresets, presetName);
            var actualSavePath = savePath;

            for (var i = 0; File.Exists(actualSavePath); i++)
            {
                actualSavePath = $"{savePath}_{i}";
                i++;
            }

            actualSavePath += FILENAME_EXTENSION;

            File.WriteAllText(actualSavePath,
                JsonConvert.SerializeObject(Settings, StaticSettings.JsonSerializerSettings));
            return actualSavePath;
        }

        public bool TryLoad(string presetName)
        {
            var path = Path.Combine(CustomizationPaths.GameSettingsPresets, presetName);
            path += FILENAME_EXTENSION;
            if (!File.Exists(path)) return false;

            Settings = JsonConvert.DeserializeObject<SettingsContainer>(File.ReadAllText(path),
                StaticSettings.JsonSerializerSettings);

            SettingsReloaded?.Invoke();
            return true;
        }

        public void SetValue<T>(T value, string[] path)
        {
            if (path.Length == 0) return;
            MemberInfo memberInfo = null;
            object oldObject = null;
            object obj = Settings;
            var type = obj.GetType();
            foreach (var fieldName in path)
            {
                var memberInfos = type.GetMember(fieldName, MemberTypes.Field | MemberTypes.Property,
                    BindingFlags.Default);
                if (memberInfos.Length <= 0) return;
                memberInfo = memberInfos[0];

                oldObject = obj;
                obj = memberInfo switch
                {
                    FieldInfo fieldInfo => fieldInfo.GetValue(obj),
                    PropertyInfo propertyInfo => propertyInfo.GetValue(obj),
                    _ => null
                };

                if (obj == null) return;

                type = obj.GetType();
            }

            if (type != typeof(T)) return;

            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    fieldInfo.SetValue(oldObject, value);
                    break;
                case PropertyInfo propertyInfo:
                    propertyInfo.SetValue(oldObject, value);
                    break;
            }
        }

        public T GetValue<T>(string[] path)
        {
            if (path.Length == 0) return default;
            object obj = Settings;
            var type = obj.GetType();
            foreach (var fieldName in path)
            {
                var memberInfos = type.GetMember(fieldName, MemberTypes.Field | MemberTypes.Property,
                    BindingFlags.Default);
                if (memberInfos.Length <= 0) return default;
                var memberInfo = memberInfos[0];

                obj = memberInfo switch
                {
                    FieldInfo fieldInfo => fieldInfo.GetValue(obj),
                    PropertyInfo propertyInfo => propertyInfo.GetValue(obj),
                    _ => null
                };

                if (obj == null) return default;

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
                var memberInfos = type.GetMember(fieldName, MemberTypes.Field | MemberTypes.Property,
                    BindingFlags.Default);
                if (memberInfos.Length <= 0) return false;
                var memberInfo = memberInfos[0];

                obj = memberInfo switch
                {
                    FieldInfo fieldInfo => fieldInfo.GetValue(obj),
                    PropertyInfo propertyInfo => propertyInfo.GetValue(obj),
                    _ => null
                };

                if (obj == null) return false;

                type = obj.GetType();
            }

            return type == typeof(T);
        }

        [Serializable]
        public record SettingsContainer
        {
            public GeneralSettings General = new();
            public ControlsSettings Controls = new();
            public BoardDimensionsSettings BoardDimensions = new();
            public GravitySettings Gravity => new();
            public ObjectiveSettings Objective = new();
            public PresentationSettings Presentation = new();
        }
    }
}