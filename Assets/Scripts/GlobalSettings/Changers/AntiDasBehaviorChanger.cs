using Blockstacker.GlobalSettings.Enums;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class AntiDasBehaviorChanger : AppSettingChangerBase<AntiDasBehavior>
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;

        private static AntiDasBehavior[] Values => new[]
        {
            AntiDasBehavior.DontCancel,
            AntiDasBehavior.CancelFirstDirection,
            AntiDasBehavior.CancelBothDirections
        };

        private static string[] ShownValues => new[]
        {
            "Don't cancel",
            "Cancel first direction",
            "Cancel both directions"
        };

        private void Start()
        {
            _dropdown.ClearOptions();
            for (var i = 0; i < Values.Length; i++)
            {
                var value = Values[i];
                _dropdown.options.Add(new TMP_Dropdown.OptionData(ShownValues[i]));
                if (value == AppSettings.Handling.AntiDasBehavior) _dropdown.SetValueWithoutNotify(i);
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