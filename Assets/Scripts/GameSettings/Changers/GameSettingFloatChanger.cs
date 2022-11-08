using System.Globalization;
using UnityEngine;

namespace Blockstacker.GameSettings.Changers
{
    public class GameSettingFloatChanger : GameSettingChangerWithField<float>
    {
        [Space] [SerializeField] private bool _clampValue;

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
            if (!float.TryParse(value, out var floatValue))
            {
                RefreshValue();
                return;
            }

            var actualValue = _clampValue ? Mathf.Clamp(floatValue, _minValue, _maxValue) : floatValue;
            SetValue(actualValue);
            _valueField.SetTextWithoutNotify(actualValue.ToString(CultureInfo.InvariantCulture));
        }
    }
}