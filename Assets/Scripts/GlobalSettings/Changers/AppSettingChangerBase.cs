using System;
using Blockstacker.Common.Extensions;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public abstract class AppSettingChangerBase<T> : MonoBehaviour, ISettingChanger
    {
        [SerializeField] protected string[] _controlPath = new string[0];
        [SerializeField] private TMP_Text _title;
        [SerializeField] private bool _autoformatName = true;

        protected void OnValidate()
        {
            if (_title == null) return;
            if (!AppSettings.SettingExists<T>(_controlPath))
                _title.text = "Setting not found!";
            else if (_autoformatName)
                _title.text = _controlPath[^1].FormatCamelCase();
        }

        public event Action SettingChanged;
        public static event Action<string> SettingChangedWithName;

        protected void OnSettingChanged()
        {
            SettingChanged?.Invoke();
            SettingChangedWithName?.Invoke(string.Join("/", _controlPath));
        }

        public void SetValue(T value)
        {
            if (AppSettings.TrySetValue(value, _controlPath))
            {
                SettingChanged?.Invoke();
                SettingChangedWithName?.Invoke(string.Join("/", _controlPath));
            }
        }
    }
}