using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Blockstacker.GlobalSettings.Changers
{
    public class AppSettingFloatChanger : AppSettingChangerBase<float>
    {
        [Space]
        [SerializeField] private TMP_InputField _valueField;
        [SerializeField] private BetterSlider _slider;
        [Header("Slider settings")]
        [SerializeField] private float _range = 100;
        [SerializeField] private float _minValue = 0;
        [SerializeField] private float _maxValue = 100;
        [SerializeField] private float _multiplier = 1;
        [Header("Other settings")]
        [SerializeField] private bool _clampValue = true;
        [SerializeField] private bool _maxIsInfinity = false;
        [SerializeField] private string _infinityString = "INF";
        [SerializeField] private UnityEvent<float> _valueChanged;


        private void Start()
        {
            OnValidate();
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
            if (value == float.PositiveInfinity) return _infinityString;
            return Math.Round(value * _multiplier, 2).ToString().Replace('.', ',');
        }

        public void OnValueRewritten(string value)
        {
            if (string.IsNullOrEmpty(value)) value = "0";
            var newValue = float.Parse(value);
            newValue /= _multiplier;
            if (_clampValue) {
                newValue = Mathf.Clamp(newValue, _minValue, _maxValue);
                _valueField.SetTextWithoutNotify(FormatValue(newValue));
            }
            _slider.SetRealValue(newValue);
            SetValue(newValue);
            _valueChanged.Invoke(newValue);
            OnSettingChanged();
        }

        public void OnSliderMoved()
        {
            var value = _slider.GetRealValue();
            if (value == _maxValue && _maxIsInfinity) {
                value = float.PositiveInfinity;
                _valueField.SetTextWithoutNotify(_infinityString);
            }
            else {
                _valueField.SetTextWithoutNotify(FormatValue(value));
            }
            SetValue(value);
            _valueChanged.Invoke(value);
            OnSettingChanged();
        }

    }
}
