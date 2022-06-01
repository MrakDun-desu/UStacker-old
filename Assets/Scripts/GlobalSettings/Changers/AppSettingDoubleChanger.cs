using System;
using System.Globalization;
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
        private bool _clampValue = true;

        [SerializeField] private bool _maxIsInfinity;
        [SerializeField] private string _infinityString = "INF";
        [SerializeField] private UnityEvent<float> _valueChanged;


        private void Start()
        {
            OnValidate();
            var value = AppSettings.GetValue<double>(_controlPath);
            _slider.SetRealValue((float) value);
            _valueField.SetTextWithoutNotify(FormatValue(value));
            _valueChanged.Invoke((float) value);
        }

        private new void OnValidate()
        {
            base.OnValidate();
            _slider.MaxValue = (float) _maxValue;
            _slider.MinValue = (float) _minValue;
            _slider.Range = (float) _range;
        }

        private string FormatValue(double value)
        {
            return double.IsPositiveInfinity(value)
                ? _infinityString
                : Math.Round(value * _multiplier, 2).ToString(CultureInfo.InvariantCulture).Replace('.', ',');
        }

        public void OnValueRewritten(string value)
        {
            if (string.IsNullOrEmpty(value)) value = "0";
            var newValue = double.Parse(value);
            newValue /= _multiplier;
            if (_clampValue)
            {
                newValue = Math.Clamp(newValue, _minValue, _maxValue);
                _valueField.SetTextWithoutNotify(FormatValue(newValue));
            }

            _slider.SetRealValue((float) newValue);
            SetValue(newValue);
            _valueChanged.Invoke((float) newValue);
            OnSettingChanged();
        }

        public void OnSliderMoved()
        {
            var value = (double) _slider.GetRealValue();
            if (Math.Abs(value - _maxValue) < .1f && _maxIsInfinity)
            {
                value = double.PositiveInfinity;
                _valueField.SetTextWithoutNotify(_infinityString);
            }
            else
            {
                _valueField.SetTextWithoutNotify(FormatValue(value));
            }

            SetValue(value);
            _valueChanged.Invoke((float) value);
            OnSettingChanged();
        }
    }
}