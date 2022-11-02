using UnityEngine;

namespace Blockstacker.GameSettings.Changers
{
    public class GameSettingUIntChanger : GameSettingChangerWithField<uint>
    {
        [Space] [SerializeField] private bool _clampValue;

        [SerializeField] private uint _maxValue;
        [SerializeField] private uint _minValue;

        protected override void OnValidate()
        {
            base.OnValidate();
            if (_minValue > _maxValue) _minValue = _maxValue;
        }

        protected override void OnValueOverwritten(string value)
        {
            if (!uint.TryParse(value, out var uintValue))
            {
                RefreshValue();
                return;
            }
            if (_clampValue)
            {
                if (uintValue < _minValue) uintValue = _minValue;
                if (uintValue > _maxValue) uintValue = _maxValue;
            }

            SetValue(uintValue);
            _valueField.SetTextWithoutNotify(uintValue.ToString());
        }
    }
}