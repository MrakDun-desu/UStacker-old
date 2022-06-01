using Blockstacker.GameSettings.Enums;
using TMPro;
using UnityEngine;

namespace Blockstacker.GameSettings.Changers
{
    public class OnTouchGroundChanger : GameSettingChangerBase<OnTouchGround>
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;

        private static OnTouchGround[] Values => new[]
        {
            OnTouchGround.InfiniteMovement,
            OnTouchGround.LimitedMoves,
            OnTouchGround.LimitedTime
        };

        private static string[] ShownValues => new[]
        {
            "Infinite movement",
            "Limited moves",
            "Limited time"
        };

        private void Start()
        {
            _dropdown.ClearOptions();
            for (var i = 0; i < Values.Length; i++)
            {
                var value = Values[i];
                _dropdown.options.Add(new TMP_Dropdown.OptionData(ShownValues[i]));
                if (value == _gameSettingsSO.Rules.Controls.OnTouchGround) _dropdown.SetValueWithoutNotify(i);
            }

            _dropdown.RefreshShownValue();
        }

        public void OnValuePicked(int index)
        {
            SetValue(Values[index]);
        }
    }
}