using System;
using Blockstacker.GlobalSettings.Loaders;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class SkinChanger : MonoBehaviour, ISettingChanger
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;

        [SerializeField] private string _emptyPrompt = "No skin available";

        private void Start()
        {
            RefreshNames();
        }

        public event Action SettingChanged;

        public void OptionPicked(int value)
        {
            var newSkin = _dropdown.options[value].text;
            if (newSkin.Equals(_emptyPrompt)) return;
            AppSettings.Customization.SkinFolder = newSkin;
            SettingChanged?.Invoke();
        }

        public void RefreshNames()
        {
            _dropdown.ClearOptions();
            var i = 0;
            foreach (var path in SkinLoader.EnumerateSkins())
            {
                _dropdown.options.Add(new TMP_Dropdown.OptionData(path));
                if (path.Equals(AppSettings.Customization.SkinFolder)) _dropdown.SetValueWithoutNotify(i);
                i++;
            }

            if (i == 0) _dropdown.options.Add(new TMP_Dropdown.OptionData(_emptyPrompt));
            _dropdown.RefreshShownValue();
        }
    }
}