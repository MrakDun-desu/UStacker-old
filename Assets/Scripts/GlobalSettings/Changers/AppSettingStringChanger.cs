using TMPro;
using UnityEngine;

namespace UStacker.GlobalSettings.Changers
{
    public class AppSettingStringChanger : AppSettingChangerBase<string>
    {
        [Space] [SerializeField] private TMP_InputField _field;

        private void Start()
        {
            RefreshValue();
            AppSettings.SettingsReloaded += RefreshValue;
            _field.onEndEdit.AddListener(SetValue);
        }

        private void RefreshValue()
        {
            _field.text = AppSettings.GetValue<string>(_controlPath);
        }
    }
}