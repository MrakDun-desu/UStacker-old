using System;
using System.Globalization;
using System.Linq;
using Blockstacker.Common.UIToolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.ChangersUIToolkit
{
    public class ResolutionChanger : VisualElement, INotifyOnChange
    {
        public new class UxmlFactory : UxmlFactory<ResolutionChanger>
        {
        }

        public event Action Changed;

        private readonly Vector2Int[] _resolutions;
        private readonly RefreshRate[] _refreshRates;
        private readonly BsDropdownField _resolutionDropdown;
        private readonly BsDropdownField _refreshRateDropdown;

        public ResolutionChanger()
        {
            _resolutions = Screen.resolutions
                .Select(res => new Vector2Int(res.width, res.height))
                .Distinct().ToArray();

            _refreshRates = Screen.resolutions
                .Select(res => res.refreshRateRatio)
                .Distinct().ToArray();

            _resolutionDropdown = new BsDropdownField("Resolution");
            _refreshRateDropdown = new BsDropdownField("Refresh rate");

            foreach (var resolution in _resolutions)
                _resolutionDropdown.Choices.Add(
                    new BsDropdownField.ChoiceWithTooltip($"{resolution.x} x {resolution.y}"));

            foreach (var refreshRate in _refreshRates)
                _refreshRateDropdown.Choices.Add(
                    new BsDropdownField.ChoiceWithTooltip(refreshRate.value.ToString(CultureInfo.InvariantCulture)));

            var currentRes = Screen.currentResolution;
            _resolutionDropdown.value = $"{currentRes.width} x {currentRes.height}";
            _refreshRateDropdown.value = currentRes.refreshRateRatio.value.ToString(CultureInfo.InvariantCulture);

            _resolutionDropdown.RegisterValueChangedCallback(OnResolutionChanged);
            _refreshRateDropdown.RegisterValueChangedCallback(OnRefreshRateChanged);
            
            Add(_resolutionDropdown);
            Add(_refreshRateDropdown);
        }

        private void OnResolutionChanged(ChangeEvent<string> _)
        {
            var newValue = _resolutions[_resolutionDropdown.Index];
            var newResolution = new Resolution
            {
                width = newValue.x,
                height = newValue.y, 
                refreshRateRatio = Screen.currentResolution.refreshRateRatio
            };

            AppSettings.Video.Resolution = newResolution;
            Changed?.Invoke();
        }

        private void OnRefreshRateChanged(ChangeEvent<string> _)
        {
            var newValue = _refreshRates[_refreshRateDropdown.Index];
            var newResolution = new Resolution
            {
                width = Screen.currentResolution.width,
                height = Screen.currentResolution.height,
                refreshRateRatio = newValue
            };

            AppSettings.Video.Resolution = newResolution;
            Changed?.Invoke();
        }
    }
}