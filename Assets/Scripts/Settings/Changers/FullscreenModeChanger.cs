using TMPro;
using UnityEngine;

namespace Blockstacker.Settings.Changers
{
    public class FullscreenModeChanger : AppSettingChangerBase<string>
    {
        [Space]
        [SerializeField] private TMP_Dropdown _dropdown;

        private static string[] Values => new string[] {
            "ExclusiveFullScreen",
            "FullScreenWindow",
            "MaximizedWindow",
            "Windowed"
        };

        private void Start()
        {
            _dropdown.ClearOptions();
            for (int i = 0; i < Values.Length; i++) {
                string value = Values[i];
                _dropdown.options.Add(new TMP_Dropdown.OptionData(value));
                if (value.Equals(AppSettings.Video.FullscreenMode)) {
                    _dropdown.SetValueWithoutNotify(i);
                }
            }
            _dropdown.RefreshShownValue();
        }
    }
}