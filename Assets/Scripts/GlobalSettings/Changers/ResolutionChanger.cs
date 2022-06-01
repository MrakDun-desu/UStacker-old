using System;
using Blockstacker.Common.Extensions;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class ResolutionChanger : MonoBehaviour, ISettingChanger
    {
        [SerializeField] private TMP_Dropdown _dropdown;
        private Resolution[] _resolutions = Array.Empty<Resolution>();

        private void Start()
        {
            _resolutions = Screen.resolutions;
            _dropdown.ClearOptions();
            for (var i = 0; i < _resolutions.Length; i++)
            {
                var resolution = _resolutions[i];
                _dropdown.options.Add(new TMP_Dropdown.OptionData(resolution.ToString()));
                if (resolution.IsEqualTo(Screen.currentResolution)) _dropdown.SetValueWithoutNotify(i);
            }

            _dropdown.RefreshShownValue();
        }

        public event Action SettingChanged;

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