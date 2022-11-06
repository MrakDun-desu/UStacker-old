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
        [SerializeField] protected string _emptyPrompt = string.Empty;
        [SerializeField] private Button _openDirectoryButton;
        
        protected abstract string TargetDir { get; }
        protected abstract string DefaultEmptyPrompt { get; }
        
        private new void OnValidate() {
            base.OnValidate();
            if (string.IsNullOrEmpty(_emptyPrompt))
                _emptyPrompt = DefaultEmptyPrompt;
        }

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

            if (string.IsNullOrEmpty(optionName) || optionName == _emptyPrompt)
                return;
            
            SetValue(optionName);
        }

        private void RefreshValue()
        {
            _dropdown.ClearOptions();
            _dropdown.options.Add(new TMP_Dropdown.OptionData(string.Empty));
            _dropdown.SetValueWithoutNotify(0);

            var options = Directory.EnumerateFiles(TargetDir).Select(Path.GetFileNameWithoutExtension).ToArray();
            for (var i = 0; i < options.Length; i++)
            {
                var optionName = options[i];
                _dropdown.options.Add(new TMP_Dropdown.OptionData(optionName));
                if (optionName == _gameSettingsSO.GetValue<string>(_controlPath))
                    _dropdown.SetValueWithoutNotify(i);
            }
            
            if (_dropdown.options.Count == 1) 
                _dropdown.options.Add(new TMP_Dropdown.OptionData(_emptyPrompt));
            
            _dropdown.RefreshShownValue();
        }

    }
}