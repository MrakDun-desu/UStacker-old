using UnityEngine;

namespace Blockstacker.GameSettings.Changers
{
    public class GameSettingUintChanger : GameSettingChangerWithField<uint>
    {
        [Space]
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

            SetValue(uintValue);
            var actualValue = _gameSettingsSO.GetValue<uint>(_controlPath);
            _valueField.SetTextWithoutNotify(actualValue.ToString());
        }
    }
}