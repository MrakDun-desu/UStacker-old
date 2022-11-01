using Blockstacker.GlobalSettings.Backgrounds;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class BackgroundPackChanger : AppSettingChangerBase<string>
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;

        [SerializeField] private string _default = "Default";

        private void Start()
        {
            RefreshNames();
            
            AppSettings.SettingsReloaded += RefreshValue;
            _dropdown.onValueChanged.AddListener(OnOptionPicked);
        }

        public void RefreshNames()
        {
            _dropdown.ClearOptions();
            _dropdown.options.Add(new TMP_Dropdown.OptionData(_default));
            _dropdown.SetValueWithoutNotify(0);
            foreach (var path in BackgroundPackLoader.EnumerateBackgroundPacks())
                _dropdown.options.Add(new TMP_Dropdown.OptionData(path));

            RefreshValue();
        }

        private void RefreshValue()
        {
            for (var i = 0; i < _dropdown.options.Count; i++)
            {
                if (!_dropdown.options[i].text.Equals(AppSettings.GetValue<string>(_controlPath))) continue;
                
                _dropdown.SetValueWithoutNotify(i);
                break;
            }
            _dropdown.RefreshShownValue();
        }

        private void OnOptionPicked(int value)
        {
            var newBackgroundFolder = _dropdown.options[value].text;

            if (newBackgroundFolder.Equals(_default))
                newBackgroundFolder = "";
            
            SetValue(newBackgroundFolder);
        }
    }
}