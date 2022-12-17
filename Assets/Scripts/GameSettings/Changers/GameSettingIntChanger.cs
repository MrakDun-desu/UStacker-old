using UnityEngine;

namespace UStacker.GameSettings.Changers
{
    public class GameSettingIntChanger : GameSettingChangerWithField<int>
    {
        [Space]
        [SerializeField] private int _maxValue;
        [SerializeField] private int _minValue;

        protected override void OnValidate()
        {
            base.OnValidate();
            if (_minValue > _maxValue) _minValue = _maxValue;
        }

        protected override void OnValueOverwritten(string value)
        {
            if (!int.TryParse(value, out var intValue))
            {
                RefreshValue();
                return;
            }

            SetValue(intValue);
            var actualValue = _gameSettingsSO.GetValue<int>(_controlPath);
            _valueField.SetTextWithoutNotify(actualValue.ToString());
        }
    }
}