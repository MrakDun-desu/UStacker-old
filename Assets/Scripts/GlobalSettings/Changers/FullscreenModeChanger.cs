using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class FullscreenModeChanger : AppSettingChangerBase<string>
    {
        [Space] [SerializeField] private TMP_Dropdown _dropdown;

        private static string[] Values => new[]
        {
            nameof(FullScreenMode.ExclusiveFullScreen),
            nameof(FullScreenMode.FullScreenWindow),
            nameof(FullScreenMode.Windowed)
        };

        private static string[] ShownValues => new[]
        {
            "Exclusive fullscreen",
            "Fullscreen window",
            "Windowed"
        };

        private void Start()
        {
            _dropdown.ClearOptions();
            for (var i = 0; i < Values.Length; i++)
            {
                var value = Values[i];
                _dropdown.options.Add(new TMP_Dropdown.OptionData(ShownValues[i]));
                if (value.Equals(AppSettings.Video.FullscreenMode)) _dropdown.SetValueWithoutNotify(i);
            }

            _dropdown.RefreshShownValue();
        }

        public void OnValuePicked(int index)
        {
            AppSettings.Video.FullscreenMode = Values[index];
            OnSettingChanged();
        }
    }
}