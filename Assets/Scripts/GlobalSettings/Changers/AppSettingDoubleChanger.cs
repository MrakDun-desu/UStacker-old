using System;
using System.Globalization;
using Blockstacker.Common.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Blockstacker.GlobalSettings.Changers
{
    public class AppSettingDoubleChanger : AppSettingChangerBase<double>
    {
        [Space] [SerializeField] private TMP_InputField _valueField;
        [SerializeField] private BetterSlider _slider;

        [Header("Slider settings")] [SerializeField]
        private double _range = 100;

        [SerializeField] private double _minValue;
        [SerializeField] private double _maxValue = 100;
        [SerializeField] private double _multiplier = 1;

        [Header("Other settings")] [SerializeField]
        private bool _maxIsInfinity;
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

        private new void OnValidate()
        {
            base.OnValidate();
            _slider.MaxValue = (float) _maxValue;
            _slider.MinValue = (float) _minValue;
            _slider.Range = (float) _range;
        }

        private void RefreshValue()
        {
            var value = AppSettings.GetValue<double>(_controlPath);
            _slider.SetRealValue((float) value);
            _valueField.SetTextWithoutNotify(FormatValue(value));
            _valueChanged.Invoke((float) value);
        }

        private string FormatValue(double value)
        {
            return double.IsPositiveInfinity(value)
                ? _infinityString
                : Math.Round(value * _multiplier, 2).ToString(CultureInfo.InvariantCulture).Replace('.', ',');
        }

        private void OnValueRewritten(string value)
        {
            if (string.IsNullOrEmpty(value)) value = "0";

            value = value.Replace('.', ',');
            var isValid = double.TryParse(value, out var newValue);

            if (!isValid)
            {
                _valueField.SetTextWithoutNotify(FormatValue(AppSettings.GetValue<double>(_controlPath)));
                return;
            }

            newValue /= _multiplier;

            SetValue(newValue);
            var actualValue = AppSettings.GetValue<double>(_controlPath);
            _slider.SetRealValue((float) actualValue);
            _valueField.SetTextWithoutNotify(double.IsPositiveInfinity(actualValue)
                ? _infinityString
                : FormatValue(actualValue));
            _valueChanged.Invoke((float) actualValue);
        }

        private void OnSliderMoved()
        {
            var value = (double) _slider.GetRealValue();
            if (Math.Abs(value - _maxValue) < float.Epsilon && _maxIsInfinity)
                value = double.PositiveInfinity;

            SetValue(value);
            var actualValue = AppSettings.GetValue<double>(_controlPath);

            _valueField.SetTextWithoutNotify(double.IsPositiveInfinity(actualValue)
                ? _infinityString
                : FormatValue(actualValue));
            _slider.SetRealValue((float) actualValue);

            _valueChanged.Invoke((float) actualValue);
        }
    }
}