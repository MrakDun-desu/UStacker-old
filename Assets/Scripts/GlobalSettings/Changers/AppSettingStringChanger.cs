using TMPro;
using UnityEngine;

namespace UStacker.GlobalSettings.Changers
{
    public class AppSettingStringChanger : AppSettingChangerBase<string>
    {
        [Space] [SerializeField] private TMP_InputField _field;

        protected override void Start()
        {
            base.Start();
            _field.onEndEdit.AddListener(SetValue);
        }

        protected override void RefreshValue()
        {
            _field.text = AppSettings.GetValue<string>(_controlPath);
        }
    }
}