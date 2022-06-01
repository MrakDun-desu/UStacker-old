using System;
using System.Reflection;
using Blockstacker.GameSettings.SettingGroups;
using UnityEngine;

namespace Blockstacker.GameSettings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Blockstacker/Game settings asset")]
    public class GameSettingsSO : ScriptableObject
    {
        [SerializeField] public SettingsContainer Settings = new();
        public RulesSettings Rules => Settings.Rules;
        public ObjectiveSettings Objective => Settings.Objective;
        public PresentationSettings Presentation => Settings.Presentation;

        public void OverrideSettings(SettingsContainer settings)
        {
            Settings = settings;
        }

        public void SetValue<T>(T value, string[] path)
        {
            if (path.Length == 0) return;
            FieldInfo fieldInfo = null;
            object oldObject = null;
            object obj = Settings;
            var type = obj.GetType();
            foreach (var fieldName in path)
            {
                fieldInfo = type.GetField(fieldName);
                if (fieldInfo == null) return;

                oldObject = obj;
                obj = fieldInfo.GetValue(obj);
                if (obj == null) return;

                type = obj.GetType();
            }

            if (type != typeof(T)) return;

            if (fieldInfo != null) fieldInfo.SetValue(oldObject, value);
        }

        public T GetValue<T>(string[] path)
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

        public bool SettingExists<T>(string[] path)
        {
            if (path.Length == 0) return false;
            object obj = Settings;
            var type = obj.GetType();
            foreach (var fieldName in path)
            {
                var fieldInfo = type.GetField(fieldName);
                if (fieldInfo == null) return false;

                obj = fieldInfo.GetValue(obj);
                if (obj == null) return false;

                type = obj.GetType();
            }

            return type == typeof(T);
        }

        [Serializable]
        public record SettingsContainer
        {
            public RulesSettings Rules = new();
            public ObjectiveSettings Objective = new();
            public PresentationSettings Presentation = new();
        }
    }
}