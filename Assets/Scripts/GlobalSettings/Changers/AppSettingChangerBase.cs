
/************************************
AppSettingChangerBase.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using TMPro;
using UnityEngine;
using UStacker.Common.Extensions;

namespace UStacker.GlobalSettings.Changers
{
    public abstract class AppSettingChangerBase<T> : MonoBehaviour, ISettingChanger
    {
        [SerializeField] protected string[] _controlPath = Array.Empty<string>();
        [SerializeField] private TMP_Text _title;
        [SerializeField] private bool _autoformatName = true;

        protected virtual void Start()
        {
            RefreshValue();
            AppSettings.SettingsReloaded += RefreshValue;
        }

        protected virtual void OnDestroy()
        {
            AppSettings.SettingsReloaded -= RefreshValue;
        }

        protected virtual void OnValidate()
        {
            if (_title == null) return;
            if (!AppSettings.SettingExists<T>(_controlPath))
                _title.text = "Setting not found!";
            else if (_autoformatName)
                _title.text = _controlPath[^1].FormatCamelCase();
        }

        public event Action SettingChanged;

        protected abstract void RefreshValue();

        protected void InvokeSettingChanged()
        {
            SettingChanged?.Invoke();
        }

        protected void SetValue(T value)
        {
            if (AppSettings.TrySetValue(value, _controlPath))
                SettingChanged?.Invoke();

            RefreshValue();
        }
    }
}
/************************************
end AppSettingChangerBase.cs
*************************************/
