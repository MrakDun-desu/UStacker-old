using System;
using Blockstacker.Loaders;
using TMPro;
using UnityEngine;

namespace Blockstacker.Settings.Changers
{
    public class SoundPackChanger : MonoBehaviour, ISettingChanger
    {
        [Space]
        [SerializeField] private TMP_Dropdown _dropdown;

        public event Action SettingChanged;

        private void Start()
        {
            RefreshNames();
        }

        public void RefreshNames()
        {
            _dropdown.ClearOptions();
            var i = 0;
            foreach (var path in SoundPackLoader.EnumerateSoundPacks()) {
                _dropdown.options.Add(new TMP_Dropdown.OptionData(path));
                if (path.Equals(AppSettings.Customization.SoundPackFolder)) {
                    _dropdown.SetValueWithoutNotify(i);
                }
                i++;
            }
            _dropdown.RefreshShownValue();
        }

        public void OptionPicked(int value)
        {
            var newSoundPack = _dropdown.options[value].text;
            AppSettings.Customization.SoundPackFolder = newSoundPack;
            SettingChanged?.Invoke();
        }
    }
}