using Blockstacker.GlobalSettings.Enums;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class DiagonalLockBehaviorChanger : AppSettingChangerBase<DiagonalLockBehavior>
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;
        
        private static DiagonalLockBehavior[] Values => new[] {
            DiagonalLockBehavior.DontLock,
            DiagonalLockBehavior.PrioritizeHorizontal,
            DiagonalLockBehavior.PrioritizeVertical
        };

        private static string[] ShownValues => new[] {
            "Don't lock",
            "Prioritize horizontal",
            "Prioritize vertical"
        };

        private void Start()
        {
            _dropdown.ClearOptions();
            for (var i = 0; i < Values.Length; i++) {
                var value = Values[i];
                _dropdown.options.Add(new TMP_Dropdown.OptionData(ShownValues[i]));
                if (value == AppSettings.Handling.DiagonalLockBehavior) {
                    _dropdown.SetValueWithoutNotify(i);
                }
            }
            _dropdown.RefreshShownValue();
        }

        public void OnValuePicked(int index)
        {
            SetValue(Values[index]);
            OnSettingChanged();
        }
    }
}