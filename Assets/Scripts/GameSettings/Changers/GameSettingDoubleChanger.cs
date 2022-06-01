using System;
using System.Globalization;
using UnityEngine;

namespace Blockstacker.GameSettings.Changers
{
    public class GameSettingDoubleChanger : GameSettingChangerWithField<double>
    {
        [Space] [SerializeField] private bool _clampValue;

        [SerializeField] private float _maxValue;
        [SerializeField] private float _minValue;

        protected override void OnValidate()
        {
            base.OnValidate();
            if (_minValue > _maxValue) _minValue = _maxValue;
        }

        public void SetValue(string value)
        {
            if (!double.TryParse(value, out var doubleValue)) return;
            SetValue(_clampValue ? Math.Clamp(doubleValue, _minValue, _maxValue) : doubleValue);
            _valueField.SetTextWithoutNotify(doubleValue.ToString(CultureInfo.InvariantCulture));
        }
    }
}