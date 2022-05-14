using Blockstacker.GameSettings.Enums;
using TMPro;
using UnityEngine;

namespace Blockstacker.GameSettings.Changers
{
    public class TopoutConditionChanger : GameSettingChangerBase<TopoutCondition>
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;
        
        private static TopoutCondition[] Values => new[] {
            TopoutCondition.PieceSpawn,
            TopoutCondition.LethalHeightLoose,
            TopoutCondition.LethalHeightStrict
        };

        private static string[] ShownValues => new[] {
            "Piece spawn",
            "Lethal height loose",
            "Lethal height strict",
        };

        private void Start()
        {
            _dropdown.ClearOptions();
            for (var i = 0; i < Values.Length; i++) {
                var value = Values[i];
                _dropdown.options.Add(new TMP_Dropdown.OptionData(ShownValues[i]));
                if (value == _gameSettingsSO.Rules.BoardDimensions.TopoutCondition) {
                    _dropdown.SetValueWithoutNotify(i);
                }
            }
            _dropdown.RefreshShownValue();
        }

        public void OnValuePicked(int index)
        {
            SetValue(Values[index]);
        }
    }
}