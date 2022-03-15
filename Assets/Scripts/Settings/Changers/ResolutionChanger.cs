using System;
using TMPro;
using UnityEngine;

namespace Blockstacker.Settings.Changers
{
    public class ResolutionChanger : MonoBehaviour, ISettingChanger
    {
        [SerializeField] private TMP_Dropdown _dropdown;
        private Resolution[] _resolutions = new Resolution[0];

        public event Action SettingChanged;

        private void Start()
        {
            _resolutions = Screen.resolutions;
            _dropdown.ClearOptions();
            for (int i = 0; i < _resolutions.Length; i++) {
                Resolution resolution = _resolutions[i];
                _dropdown.options.Add(new TMP_Dropdown.OptionData(resolution.ToString()));
                if (resolution.width == Screen.currentResolution.width &&
                    resolution.height == Screen.currentResolution.height &&
                    resolution.refreshRate == Screen.currentResolution.refreshRate) {
                    _dropdown.SetValueWithoutNotify(i);
                }
            }
            _dropdown.RefreshShownValue();
        }

        public void SetResolution(int value)
        {
            var selectedResolution = _resolutions[value];
            AppSettings.Video.Resolution = new Vector2Int(
                selectedResolution.width,
                selectedResolution.height
            );
            AppSettings.Video.RefreshRate = selectedResolution.refreshRate;
            SettingChanged?.Invoke();
        }
    }
}