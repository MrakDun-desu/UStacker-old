using UnityEngine;

namespace Blockstacker.GameSettings.Changers
{
    public class IntChanger : GameSettingChangerBase<int>
    {
        [Space]
        [SerializeField] private bool _clampValue = false;
        [SerializeField] private int _maxValue;
        [SerializeField] private int _minValue;

        protected override void OnValidate()
        {
            base.OnValidate();
            if (_minValue > _maxValue) _minValue = _maxValue;
        }

        public void SetValue(string value)
        {
            if (!int.TryParse(value, out var intValue)) return;
            if (_clampValue)
                SetValue(Mathf.Clamp(intValue, _minValue, _maxValue));
            else
                SetValue(intValue);
        }
    }
}