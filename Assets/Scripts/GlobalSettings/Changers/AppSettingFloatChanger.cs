using System;
using System.Globalization;
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

        [Header("Other settings")] [SerializeField]
        private bool _clampValue = true;

        [SerializeField] private bool _maxIsInfinity;
        [SerializeField] private string _infinityString = "INF";
        [SerializeField] private UnityEvent<float> _valueChanged;


        private void Start()
        {
            OnValidate();
            
            RefreshValue();
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

        public void OnValueRewritten(string value)
        {
            if (string.IsNullOrEmpty(value)) value = "0";
            var newValue = float.Parse(value);
            newValue /= _multiplier;
            if (_clampValue)
            {
                newValue = Mathf.Clamp(newValue, _minValue, _maxValue);
                _valueField.SetTextWithoutNotify(FormatValue(newValue));
            }

            _slider.SetRealValue(newValue);
            SetValue(newValue);
            _valueChanged.Invoke(newValue);
            InvokeSettingChanged();
        }

        public void OnSliderMoved()
        {
            var value = _slider.GetRealValue();
            if (Mathf.Abs(value - _maxValue) < .1f && _maxIsInfinity)
            {
                value = float.PositiveInfinity;
                _valueField.SetTextWithoutNotify(_infinityString);
            }
            else
            {
                _valueField.SetTextWithoutNotify(FormatValue(value));
            }

            SetValue(value);
            _valueChanged.Invoke(value);
            InvokeSettingChanged();
        }
    }
}