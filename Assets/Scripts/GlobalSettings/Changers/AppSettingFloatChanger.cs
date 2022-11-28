using System;
using System.Globalization;
using Blockstacker.Common.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Blockstacker.GlobalSettings.Changers
{
    public class AppSettingFloatChanger : AppSettingChangerBase<float>
    {
        [Space] [SerializeField] private TMP_InputField _valueField;

        [SerializeField] private BetterSlider _slider;

        [Header("Slider settings")] [SerializeField]
        private float _range = 100;

        [SerializeField] private float _minValue;
        [SerializeField] private float _maxValue = 100;
        [SerializeField] private float _multiplier = 1;

        [Header("Other settings")]
        [SerializeField] private bool _maxIsInfinity;
        [SerializeField] private string _infinityString = "INF";
        [SerializeField] private UnityEvent<float> _valueChanged;


        private void Start()
        {
            OnValidate();
            
            RefreshValue();

            _slider.ValueChanged += _ => OnSliderMoved();
            _valueField.onEndEdit.AddListener(OnValueRewritten);
            AppSettings.SettingsReloaded += RefreshValue;
        }

        private void RefreshValue()
        {
            var value = AppSettings.GetValue<float>(_controlPath);
            _slider.SetRealValue(value);
            _valueField.SetTextWithoutNotify(FormatValue(value));
            _valueChanged.Invoke(value);
        }

        private new void OnValidate()
        {
            base.OnValidate();
            _slider.MaxValue = _maxValue;
            _slider.MinValue = _minValue;
            _slider.Range = _range;
        }

        private string FormatValue(float value)
        {
            if (float.IsPositiveInfinity(value)) return _infinityString;
            return Math.Round(value * _multiplier, 2).ToString(CultureInfo.InvariantCulture).Replace('.', ',');
        }

        private void OnValueRewritten(string value)
        {
            if (string.IsNullOrEmpty(value)) value = "0";
            
            value = value.Replace('.', ',');
            var isValid = float.TryParse(value, out var newValue);

            if (!isValid)
            {
                _valueField.SetTextWithoutNotify(FormatValue(AppSettings.GetValue<float>(_controlPath)));
                return;
            }
            
            newValue /= _multiplier;

            SetValue(newValue);
            var actualValue = AppSettings.GetValue<float>(_controlPath);
            _slider.SetRealValue(actualValue);
            _valueField.SetTextWithoutNotify(float.IsPositiveInfinity(actualValue)
                ? _infinityString
                : FormatValue(actualValue));
            _valueChanged.Invoke(actualValue);
        }

        private void OnSliderMoved()
        {
            var value = _slider.GetRealValue();
            if (Mathf.Abs(value - _maxValue) < float.Epsilon && _maxIsInfinity)
                value = float.PositiveInfinity;

            SetValue(value);
            var actualValue = AppSettings.GetValue<float>(_controlPath);
            _slider.SetRealValue(actualValue);
            _valueField.SetTextWithoutNotify(float.IsPositiveInfinity(actualValue)
                ? _infinityString
                : FormatValue(actualValue));
            _valueChanged.Invoke(actualValue);
        }
    }
}