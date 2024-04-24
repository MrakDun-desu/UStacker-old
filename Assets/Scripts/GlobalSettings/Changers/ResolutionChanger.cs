
/************************************
ResolutionChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;

namespace UStacker.GlobalSettings.Changers
{
    public class ResolutionChanger : AppSettingChangerBase<Resolution>
    {
        [SerializeField] private TMP_Dropdown _resolutionDropdown;
        [SerializeField] private TMP_Dropdown _refreshRateDropdown;
        private RefreshRate[] _refreshRates = Array.Empty<RefreshRate>();
        private Vector2Int[] _resolutions = Array.Empty<Vector2Int>();

        private void Awake()
        {
            RefreshResolutions();
        }

        protected override void Start()
        {
            base.Start();

            _resolutionDropdown.onValueChanged.AddListener(OnResolutionPicked);
            _refreshRateDropdown.onValueChanged.AddListener(OnRefreshRatePicked);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                return;

            RefreshResolutions();
        }

        private void RefreshResolutions()
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

            _refreshRateDropdown.ClearOptions();
            foreach (var refreshRate in _refreshRates)
                _refreshRateDropdown.options.Add(
                    new TMP_Dropdown.OptionData(refreshRate.value.ToString(CultureInfo.InvariantCulture)));
        }

        protected override void RefreshValue()
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

        private void OnResolutionPicked(int value)
        {
            var newVal = _resolutions[value];
            var newRes = new Resolution
            {
                height = newVal.y, width = newVal.x, refreshRateRatio = Screen.currentResolution.refreshRateRatio
            };
            SetValue(newRes);
        }

        private void OnRefreshRatePicked(int value)
        {
            var newRes = new Resolution
            {
                height = Screen.currentResolution.height, width = Screen.currentResolution.width,
                refreshRateRatio = _refreshRates[value]
            };
            SetValue(newRes);
        }
    }
}
/************************************
end ResolutionChanger.cs
*************************************/
