using Blockstacker.GameSettings.Enums;
using TMPro;
using UnityEngine;

namespace Blockstacker.GameSettings.Changers
{
    public class RandomizerTypeChanger : GameSettingChangerBase<RandomizerType>
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;

        private static RandomizerType[] Values => new[]
        {
            RandomizerType.SevenBag,
            RandomizerType.FourteenBag,
            RandomizerType.Classic,
            RandomizerType.Random,
            RandomizerType.Pairs
        };

        private static string[] ShownValues => new[]
        {
            "Seven bag",
            "Fourteen bag",
            "Classic",
            "Random",
            "Pairs"
        };

        private void Start()
        {
            _dropdown.ClearOptions();
            for (var i = 0; i < Values.Length; i++)
            {
                var value = Values[i];
                _dropdown.options.Add(new TMP_Dropdown.OptionData(ShownValues[i]));
                if (value == _gameSettingsSO.Rules.General.RandomizerType) _dropdown.SetValueWithoutNotify(i);
            }

            _dropdown.RefreshShownValue();
        }

        public void OnValuePicked(int index)
        {
            SetValue(Values[index]);
        }
    }
}