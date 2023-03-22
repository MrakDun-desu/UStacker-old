using UnityEngine;
using UnityEngine.UI;

namespace UStacker.GlobalSettings.Changers
{
    public class AppSettingBoolChanger : AppSettingChangerBase<bool>
    {
        [Space] [SerializeField] private Toggle _toggle;

        protected override void Start()
        {
            base.Start();
            _toggle.onValueChanged.AddListener(SetValue);
        }

        protected override void RefreshValue()
        {
            _toggle.isOn = AppSettings.GetValue<bool>(_controlPath);
        }
    }
}