using System.Globalization;
using UnityEngine;

namespace Blockstacker.GameSettings.Changers
{
    public class GameSettingFloatChanger : GameSettingChangerWithField<float>
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
            _valueField.SetTextWithoutNotify(_gameSettingsSO.GetValue<float>(_controlPath).ToString(CultureInfo.InvariantCulture));
        }

        protected override void OnValueOverwritten(string value)
        {
            value = value.Replace('.', ',');
            if (!float.TryParse(value, out var floatValue))
            {
                RefreshValue();
                return;
            }

            SetValue(floatValue);

            var actualValue = _gameSettingsSO.GetValue<float>(_controlPath);
            _valueField.SetTextWithoutNotify(actualValue.ToString(CultureInfo.InvariantCulture));
        }
    }
}