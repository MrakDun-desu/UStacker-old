using Blockstacker.GameSettings.Enums;
using TMPro;
using UnityEngine;

namespace Blockstacker.GameSettings.Changers
{
    public class MainStatChanger : GameSettingChangerBase<MainStat>
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;

        private static MainStat[] Values => new[]
        {
            MainStat.Time,
            MainStat.LinesCleared,
            MainStat.PiecesUsed
        };

        private static string[] ShownValues => new[]
        {
            "Time",
            "Lines cleared",
            "Pieces used"
        };

        private void Start()
        {
            _dropdown.ClearOptions();
            for (var i = 0; i < Values.Length; i++)
            {
                var value = Values[i];
                _dropdown.options.Add(new TMP_Dropdown.OptionData(ShownValues[i]));
                if (value == _gameSettingsSO.Objective.MainStat) _dropdown.SetValueWithoutNotify(i);
            }

            _dropdown.RefreshShownValue();
        }

        public void OnValuePicked(int index)
        {
            SetValue(Values[index]);
        }
    }
}