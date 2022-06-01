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

        public void SetValue(string value)
        {
            if (!float.TryParse(value, out var floatValue)) return;
            SetValue(_clampValue ? Mathf.Clamp(floatValue, _minValue, _maxValue) : floatValue);
            _valueField.SetTextWithoutNotify(floatValue.ToString(CultureInfo.InvariantCulture));
        }
    }
}