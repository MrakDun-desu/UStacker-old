
/************************************
AppSettingFloatChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UStacker.Common.Extensions;
using UStacker.Common.UI;

namespace UStacker.GlobalSettings.Changers
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
        private bool _maxIsInfinity;

        [SerializeField] private string _infinityString = "INF";
        [SerializeField] private UnityEvent<float> _valueChanged;


        protected override void Start()
        {
            OnValidate();
            base.Start();
            _slider.ValueChanged += OnSliderMoved;
            _valueField.onEndEdit.AddListener(OnValueRewritten);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _slider.ValueChanged -= OnSliderMoved;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            _slider.MaxValue = _maxValue;
            _slider.MinValue = _minValue;
            _slider.Range = _range;
        }

        protected override void RefreshValue()
        {
            var value = AppSettings.GetValue<float>(_controlPath);
            _slider.SetRealValue(value);
            _valueField.SetTextWithoutNotify(FormatValue(value));
            _valueChanged.Invoke(value);
        }

        private string FormatValue(float value)
        {
            return float.IsPositiveInfinity(value)
                ? _infinityString
                : Math.Round(value * _multiplier, 2).ToString(CultureInfo.InvariantCulture).Replace('.', ',');
        }

        private void OnValueRewritten(string value)
        {
            if (string.IsNullOrEmpty(value))
                value = "0";

            var isValid = value.TryParseFloat(out var newValue);

            if (!isValid)
            {
                _valueField.SetTextWithoutNotify(FormatValue(AppSettings.GetValue<float>(_controlPath)));
                return;
            }

            newValue /= _multiplier;

            SetValue(newValue);
        }

        private void OnSliderMoved(float f)
        {
            var value = _slider.GetRealValue();
            if (Mathf.Abs(value - _maxValue) < float.Epsilon && _maxIsInfinity)
                value = float.PositiveInfinity;

            SetValue(value);
        }
    }
}
/************************************
end AppSettingFloatChanger.cs
*************************************/
