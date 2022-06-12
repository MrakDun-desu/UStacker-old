using System;
using Blockstacker.Music;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class SoundPackChanger : MonoBehaviour, ISettingChanger
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;

        [SerializeField] private string _emptyPrompt = "No sound pack available";

        private void Start()
        {
            RefreshNames();
        }


        public event Action SettingChanged;

        public void RefreshNames()
        {
            _dropdown.ClearOptions();
            var i = 0;
            foreach (var path in SoundPackLoader.EnumerateSoundPacks())
            {
                _dropdown.options.Add(new TMP_Dropdown.OptionData(path));
                if (path.Equals(AppSettings.Customization.SoundPackFolder)) _dropdown.SetValueWithoutNotify(i);
                i++;
            }

            if (i == 0) _dropdown.options.Add(new TMP_Dropdown.OptionData(_emptyPrompt));
            _dropdown.RefreshShownValue();
        }

        public void OptionPicked(int value)
        {
            var newSoundPack = _dropdown.options[value].text;
            if (newSoundPack.Equals(_emptyPrompt)) return;
            AppSettings.Customization.SoundPackFolder = newSoundPack;
            SettingChanged?.Invoke();
        }
    }
}