
/************************************
GameSettingChangerBase.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using TMPro;
using UnityEngine;
using UStacker.Common.Extensions;

namespace UStacker.GameSettings.Changers
{
    public abstract class GameSettingChangerBase<T> : MonoBehaviour
    {
        [SerializeField] protected GameSettingsSO _gameSettingsSO;
        [SerializeField] protected string[] _controlPath = Array.Empty<string>();
        [SerializeField] private TMP_Text _title;
        [SerializeField] private bool _autoformatName = true;

        protected virtual void Start()
        {
            RefreshValue();
            _gameSettingsSO.SettingsReloaded += RefreshValue;
        }

        protected void OnDestroy()
        {
            _gameSettingsSO.SettingsReloaded -= RefreshValue;
        }

        protected virtual void OnValidate()
        {
            if (_gameSettingsSO == null)
                _title.text = "Choose a game settings SO!";

            if (!_gameSettingsSO.SettingExists<T>(_controlPath))
                _title.text = "Setting not found!";
            else if (_autoformatName)
                _title.text = _controlPath[^1].FormatCamelCase();
        }

        protected abstract void RefreshValue();

        protected void SetValue(T value)
        {
            _gameSettingsSO.SetValue(value, _controlPath);

            RefreshValue();
        }
    }
}
/************************************
end GameSettingChangerBase.cs
*************************************/
