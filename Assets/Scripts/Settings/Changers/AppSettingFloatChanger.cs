using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.Settings.Changers
{
    public class AppSettingFloatChanger : AppSettingChanger<float>
    {
        [Space]
        [SerializeField] private TMP_InputField _valueField;
        [SerializeField] private BetterSlider _slider;
        [Header("Slider settings")]
        [SerializeField] private float _range;
        [SerializeField] private float _minValue;
        [SerializeField] private float _maxValue;

        private void Start()
        {
            OnValidate();
            var value = AppSettings.GetValue<float>(_controlPath);
            _slider.SetRealValue(value);
            _valueField.SetTextWithoutNotify(Math.Round(value, 2).ToString());
        }

        private new void OnValidate()
        {
            base.OnValidate();
            _slider.MaxValue = _maxValue;
            _slider.MinValue = _minValue;
            _slider.Range = _range;
        }

        public void OnValueRewritten(string value)
        {
            if (string.IsNullOrEmpty(value)) value = "0";
            var newValue = float.Parse(value);
            _slider.SetRealValue(newValue);
            SetValue(newValue);
            OnSettingChanged();
        }

        public void OnSliderMoved()
        {
            var value = _slider.GetRealValue();
            _valueField.SetTextWithoutNotify(Math.Round(value, 2).ToString());
            SetValue(value);
            OnSettingChanged();
        }
    }
}
