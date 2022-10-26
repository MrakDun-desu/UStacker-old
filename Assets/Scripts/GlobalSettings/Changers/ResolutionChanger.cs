using System;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class ResolutionChanger : AppSettingChangerBase<Resolution>
    {
        [SerializeField] private TMP_Dropdown _resolutionDropdown;
        [SerializeField] private TMP_Dropdown _refreshRateDropdown;
        private Vector2Int[] _resolutions = Array.Empty<Vector2Int>();
        private RefreshRate[] _refreshRates = Array.Empty<RefreshRate>();
        
        private void Start()
        {
            _resolutions = Screen.resolutions
                .Select(res => new Vector2Int(res.width, res.height))
                .Distinct()
                .OrderByDescending(res => res.magnitude)
                .ToArray();
            
            _refreshRates = Screen.resolutions
                .Select(res => res.refreshRateRatio)
                .Distinct()
                .OrderByDescending(rate => rate.value)
                .ToArray();
            _resolutionDropdown.ClearOptions();
            foreach (var resolution in _resolutions)
                _resolutionDropdown.options.Add(new TMP_Dropdown.OptionData($"{resolution.x} x {resolution.y}"));
            
            foreach (var refreshRate in _refreshRates)
                _resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(refreshRate.value.ToString(CultureInfo.InvariantCulture)));

            RefreshValue();
            AppSettings.SettingsReloaded += RefreshValue;
        }

        private void RefreshValue()
        {
            for (var i = 0; i < _resolutionDropdown.options.Count; i++)
            {
                if (_resolutions[i].x != Screen.currentResolution.width ||
                    _resolutions[i].y != Screen.currentResolution.height) continue;
                
                _resolutionDropdown.SetValueWithoutNotify(i);
                _resolutionDropdown.RefreshShownValue();
                break;
            }

            for (var i = 0; i < _refreshRateDropdown.options.Count; i++)
            {
                if (Math.Abs(_refreshRates[i].value - Screen.currentResolution.refreshRateRatio.value) > double.Epsilon)
                    continue;

                _refreshRateDropdown.SetValueWithoutNotify(i);
                _refreshRateDropdown.RefreshShownValue();
                break;
            }
        }

        public void SetResolution(int value)
        {
            var newVal = _resolutions[value];
            var newRes = new Resolution
            {
                height = newVal.y,
                width = newVal.x,
                refreshRateRatio = Screen.currentResolution.refreshRateRatio
            };
            SetValue(newRes);
        }

        public void SetRefreshRate(int value)
        {
            var newRes = new Resolution
            {
                height = Screen.currentResolution.height,
                width = Screen.currentResolution.width,
                refreshRateRatio = _refreshRates[value]
            };
            SetValue(newRes);
        }
    }
}