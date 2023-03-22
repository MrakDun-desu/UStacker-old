using System;
using System.Globalization;
using UStacker.Common.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UStacker.Common.Extensions;

namespace UStacker.GlobalSettings.Changers
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
            _slider.MaxValue = (float) _maxValue;
            _slider.MinValue = (float) _minValue;
            _slider.Range = (float) _range;
        }

        protected override void RefreshValue()
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

            var isValid = value.TryParseDouble(out var newValue);

            if (!isValid)
            {
                _valueField.SetTextWithoutNotify(FormatValue(AppSettings.GetValue<double>(_controlPath)));
                return;
            }

            newValue /= _multiplier;

            SetValue(newValue);
        }

        private void OnSliderMoved(float _)
        {
            var value = (double) _slider.GetRealValue();
            if (Math.Abs(value - _maxValue) < float.Epsilon && _maxIsInfinity)
                value = double.PositiveInfinity;

            SetValue(value);
        }
    }
}