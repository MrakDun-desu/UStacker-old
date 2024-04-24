
/************************************
GameSettingFileChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UStacker.Common;

namespace UStacker.GameSettings.Changers
{
    public abstract class GameSettingFileChanger : GameSettingChangerBase<string>
    {
        [SerializeField] protected TMP_Dropdown _dropdown;
        [SerializeField] private Button _openDirectoryButton;

        protected abstract string TargetDir { get; }

        protected override void Start()
        {
            base.Start();
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

        protected override void RefreshValue()
        {
            const string unpickedOptionName = "Unused";
            _dropdown.ClearOptions();
            _dropdown.options.Add(new TMP_Dropdown.OptionData(unpickedOptionName));
            _dropdown.SetValueWithoutNotify(0);

            var options = Array.Empty<string>();
            if (Directory.Exists(TargetDir))
                options = Directory.EnumerateFiles(TargetDir).Select(Path.GetFileNameWithoutExtension).ToArray();

            for (var i = 0; i < options.Length; i++)
            {
                var optionName = options[i];
                _dropdown.options.Add(new TMP_Dropdown.OptionData(optionName));
                if (optionName == _gameSettingsSO.GetValue<string>(_controlPath))
                    _dropdown.SetValueWithoutNotify(i + 1);
            }

            _dropdown.RefreshShownValue();
        }
    }
}
/************************************
end GameSettingFileChanger.cs
*************************************/
