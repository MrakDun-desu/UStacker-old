using System;
using System.Globalization;
using UnityEngine;

namespace Blockstacker.GameSettings.Changers
{
    public class GameSettingDoubleChanger : GameSettingChangerWithField<double>
    {
        [Space] 
        [SerializeField] private float _maxValue;
        [SerializeField] private float _minValue;

        protected override void OnValidate()
        {
            base.OnValidate();
            if (_minValue > _maxValue) _minValue = _maxValue;
        }

        protected override void RefreshValue()
        {
            _valueField.SetTextWithoutNotify(_gameSettingsSO.GetValue<double>(_controlPath).ToString(CultureInfo.InvariantCulture));
        }

        protected override void OnValueOverwritten(string newValue)
        {
            if (!double.TryParse(newValue, out var doubleValue))
            {
                RefreshValue();
                return;
            }

            SetValue(doubleValue);
            var actualValue = _gameSettingsSO.GetValue<double>(_controlPath);
            _valueField.SetTextWithoutNotify(actualValue.ToString(CultureInfo.InvariantCulture));
        }
    }
}