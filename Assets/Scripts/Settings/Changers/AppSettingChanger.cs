using System;
using UnityEngine;

namespace Blockstacker.Settings.Changers
{
    public class AppSettingChanger<T> : MonoBehaviour, ISettingChanger
    {
        [SerializeField] protected string[] _controlPath = new string[0];

        public event Action SettingChanged;

        protected void OnValidate()
        {
            if (_controlPath.Length == 0) return;
            if (!AppSettings.SettingExists<T>(_controlPath)) {
                Debug.LogError("Setting could not be found!");
            }
        }

        public void SetValue(T value)
        {
            if (AppSettings.TrySetValue(value, _controlPath)) {
                SettingChanged?.Invoke();
            }
        }

        protected void OnSettingChanged() => SettingChanged?.Invoke();
    }
}
