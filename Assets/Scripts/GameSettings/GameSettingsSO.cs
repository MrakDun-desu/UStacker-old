using System;
using System.Reflection;
using Blockstacker.GameSettings.SettingGroups;
using UnityEngine;

namespace Blockstacker.GameSettings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Blockstacker/Game settings asset")]
    public class GameSettingsSO : ScriptableObject
    {
        [Serializable]
        internal class SettingsContainer
        {
            public RulesSettings Rules = new();
            public ObjectiveSettings Objective = new();
            public PresentationSettings Presentation = new();
        }

        [SerializeField] private SettingsContainer Settings = new();
        public RulesSettings Rules => Settings.Rules;
        public ObjectiveSettings Objective => Settings.Objective;
        public PresentationSettings Presentation => Settings.Presentation;


        public bool TrySetValue<T>(T value, string[] path)
        {
            if (path.Length == 0) return false;
            FieldInfo fieldInfo = null;
            object oldObject = null;
            object obj = Settings;
            Type type = obj.GetType();
            foreach (var fieldName in path) {
                fieldInfo = type.GetField(fieldName);
                if (fieldInfo == null) return false;

                oldObject = obj;
                obj = fieldInfo.GetValue(obj);
                if (obj == null) return false;

                type = obj.GetType();
            }

            if (type != typeof(T)) return false;

            fieldInfo.SetValue(oldObject, value);
            return true;
        }

        public T GetValue<T>(string[] path)
        {
            if (path.Length == 0) return default;
            object obj = Settings;
            Type type = obj.GetType();
            foreach (var fieldName in path) {
                FieldInfo fieldInfo = type.GetField(fieldName);
                if (fieldInfo == null) return default;

                obj = fieldInfo.GetValue(obj);
                if (obj == null) return default;

                type = obj.GetType();
            }
            if (type == typeof(T)) {
                return (T)obj;
            }
            return default;
        }

        public bool SettingExists<T>(string[] path)
        {
            if (path.Length == 0) return false;
            object obj = Settings;
            Type type = obj.GetType();
            foreach (var fieldName in path) {
                FieldInfo fieldInfo = type.GetField(fieldName);
                if (fieldInfo == null) return false;

                obj = fieldInfo.GetValue(obj);
                if (obj == null) return false;

                type = obj.GetType();
            }

            if (type != typeof(T)) return false;
            return true;
        }

    }
}