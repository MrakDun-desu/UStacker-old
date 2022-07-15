using Blockstacker.GlobalSettings.BlockSkins;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class SkinChanger : AppSettingChangerBase<string>
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;

        [SerializeField] private string _emptyPrompt = "No skin available";
        [SerializeField] private string _default = "Default";

        private void Start()
        {
            RefreshNames();

            RefreshValue();
            AppSettings.SettingsReloaded += RefreshValue;
        }

        public void OptionPicked(int value)
        {
            var newSkin = _dropdown.options[value].text;
            if (newSkin.Equals(_emptyPrompt)) return;

            if (newSkin.Equals(_default))
                newSkin = "";
            
            SetValue(newSkin);
        }

        public void RefreshNames()
        {
            _dropdown.ClearOptions();
            _dropdown.options.Add(new TMP_Dropdown.OptionData(_default));
            foreach (var path in SkinLoader.EnumerateSkins())
                _dropdown.options.Add(new TMP_Dropdown.OptionData(path));

            if (_dropdown.options.Count <= 1) 
                _dropdown.options.Add(new TMP_Dropdown.OptionData(_emptyPrompt));
        }

        private void RefreshValue()
        {
            for (var i = 0; i < _dropdown.options.Count; i++)
            {
                if (!_dropdown.options[i].text.Equals(AppSettings.GetValue<string>(_controlPath))) continue;
                
                _dropdown.SetValueWithoutNotify(i);
                _dropdown.RefreshShownValue();
                return;
            }
        }
    }
}