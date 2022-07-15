using System;
using Blockstacker.Common.Extensions;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class ResolutionChanger : AppSettingChangerBase<Resolution>
    {
        [SerializeField] private TMP_Dropdown _dropdown;
        private Resolution[] _resolutions = Array.Empty<Resolution>();
        
        private void Start()
        {
            _resolutions = Screen.resolutions;
            _dropdown.ClearOptions();
            foreach (var resolution in _resolutions)
            {
                _dropdown.options.Add(new TMP_Dropdown.OptionData(resolution.ToString()));
            }

            RefreshValue();
            AppSettings.SettingsReloaded += RefreshValue;
        }

        private void RefreshValue()
        {
            for (var i = 0; i < _dropdown.options.Count; i++)
            {
                if (!_resolutions[i].IsEqualTo(Screen.currentResolution)) continue;
                
                _dropdown.SetValueWithoutNotify(i);
                _dropdown.RefreshShownValue();
                return;
            }
        }

        public void SetResolution(int value)
        {
            SetValue(_resolutions[value]);
        }
    }
}