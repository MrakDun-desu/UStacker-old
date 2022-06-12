using System;
using Blockstacker.GlobalSettings.Loaders;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class BackgroundPackChanger : MonoBehaviour, ISettingChanger
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;

        [SerializeField] private string _emptyPrompt = "No background pack available";
        [SerializeField] private string _default = "Default";

        private void Start()
        {
            RefreshNames();
        }


        public event Action SettingChanged;

        public void RefreshNames()
        {
            _dropdown.ClearOptions();
            _dropdown.options.Add(new TMP_Dropdown.OptionData(_default));
            var i = 0;
            foreach (var path in BackgroundPackLoader.EnumerateBackgroundPacks())
            {
                _dropdown.options.Add(new TMP_Dropdown.OptionData(path));
                
                Debug.Log($"{path}, {AppSettings.Customization.BackgroundFolder}");
                
                i++;
                if (path.Equals(AppSettings.Customization.BackgroundFolder)) _dropdown.SetValueWithoutNotify(i);
            }

            if (i == 0)
            {
                _dropdown.ClearOptions();
                _dropdown.options.Add(new TMP_Dropdown.OptionData(_emptyPrompt));
            }

            _dropdown.RefreshShownValue();
        }

        public void OptionPicked(int value)
        {
            var newBackgroundFolder = _dropdown.options[value].text;
            if (newBackgroundFolder.Equals(_emptyPrompt)) return;

            if (newBackgroundFolder.Equals(_default))
                newBackgroundFolder = "";
            
            AppSettings.Customization.BackgroundFolder = newBackgroundFolder;
            SettingChanged?.Invoke();
        }
    }
}