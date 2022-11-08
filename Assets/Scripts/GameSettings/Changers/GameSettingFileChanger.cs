using System;
using System.IO;
using System.Linq;
using Blockstacker.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.GameSettings.Changers
{
    public abstract class GameSettingFileChanger : GameSettingChangerBase<string>
    {
        [SerializeField] protected TMP_Dropdown _dropdown;
        [SerializeField] private Button _openDirectoryButton;
        
        protected abstract string TargetDir { get; }
        
        private void Start()
        {
            RefreshValue();

            _gameSettingsSO.SettingsReloaded += RefreshValue;
            _dropdown.onValueChanged.AddListener(OnValuePicked);
            _openDirectoryButton.onClick.AddListener(OpenTargetDir);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus) return;
            
            RefreshValue();
        }

        private void OpenTargetDir()
        {
            DefaultAppOpener.OpenFile(TargetDir);
        }

        private void OnValuePicked(int index)
        {
            var optionName = _dropdown.options[index].text;

            if (string.IsNullOrEmpty(optionName))
                return;
            
            SetValue(optionName);
        }

        private void RefreshValue()
        {
            _dropdown.ClearOptions();
            _dropdown.options.Add(new TMP_Dropdown.OptionData(string.Empty));
            _dropdown.SetValueWithoutNotify(0);

            var options = Array.Empty<string>();
            if (Directory.Exists(TargetDir))
                options = Directory.EnumerateFiles(TargetDir).Select(Path.GetFileNameWithoutExtension).ToArray();
            
            for (var i = 0; i < options.Length; i++)
            {
                var optionName = options[i];
                _dropdown.options.Add(new TMP_Dropdown.OptionData(optionName));
                if (optionName == _gameSettingsSO.GetValue<string>(_controlPath))
                    _dropdown.SetValueWithoutNotify(i);
            }
            
            _dropdown.RefreshShownValue();
        }

    }
}