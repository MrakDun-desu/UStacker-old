using System;
using Blockstacker.Loaders;
using TMPro;
using UnityEngine;

namespace Blockstacker.Settings.Changers
{
    public class BackgroundPackChanger : MonoBehaviour, ISettingChanger
    {
        [Space]
        [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private string _emptyPrompt = "No background pack available";


        public event Action SettingChanged;

        private void Start()
        {
            RefreshNames();
        }

        public void RefreshNames()
        {
            _dropdown.ClearOptions();
            var i = 0;
            foreach (var path in BackgroundPackLoader.EnumerateBackgroundPacks()) {
                _dropdown.options.Add(new TMP_Dropdown.OptionData(path));
                if (path.Equals(AppSettings.Customization.BackgroundFolder)) {
                    _dropdown.SetValueWithoutNotify(i);
                }
                i++;
            }
            if (i == 0) {
                _dropdown.options.Add(new TMP_Dropdown.OptionData(_emptyPrompt));
            }
            _dropdown.RefreshShownValue();
        }

        public void OptionPicked(int value)
        {
            var newBackgroundFolder = _dropdown.options[value].text;
            if (newBackgroundFolder.Equals(_emptyPrompt)) return;
            AppSettings.Customization.BackgroundFolder = newBackgroundFolder;
            SettingChanged?.Invoke();
        }
    }
}