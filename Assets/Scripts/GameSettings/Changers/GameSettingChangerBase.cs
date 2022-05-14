using System;
using Blockstacker.Common.Extensions;
using TMPro;
using UnityEngine;

namespace Blockstacker.GameSettings.Changers
{
    public abstract class GameSettingChangerBase<T> : MonoBehaviour
    {
        [SerializeField] protected GameSettingsSO _gameSettingsSO;
        [SerializeField] protected string[] _controlPath = Array.Empty<string>();
        [SerializeField] private TMP_Text _title;
        [SerializeField] private bool _autoformatName = true;

        protected virtual void OnValidate()
        {
            if (_title == null) return;
            if (_gameSettingsSO == null)
                _title.text = "Choose a game settings SO!";
            
            if (!_gameSettingsSO.SettingExists<T>(_controlPath))
                _title.text = "Setting not found!";
            else if (_autoformatName)
                _title.text = _controlPath[^1].FormatCamelCase();
        }

        public void SetValue(T value)
        {
            _gameSettingsSO.TrySetValue(value, _controlPath);
        }
    }
}