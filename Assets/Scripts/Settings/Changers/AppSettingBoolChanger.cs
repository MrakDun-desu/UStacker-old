using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.Settings.Changers
{
    public class AppSettingBoolChanger : AppSettingChangerBase<bool>
    {
        [Space]
        [SerializeField] private Toggle _toggle;

        private void Start()
        {
            _toggle.isOn = AppSettings.GetValue<bool>(_controlPath);
        }
    }
}
