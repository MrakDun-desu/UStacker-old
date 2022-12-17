using UnityEngine;
using UnityEngine.UI;

namespace UStacker.GameSettings.Changers
{
    public class GameSettingBoolChanger : GameSettingChangerBase<bool>
    {
        [Space] [SerializeField] private Toggle _toggle;

        private void Start()
        {
            RefreshValue();
            _gameSettingsSO.SettingsReloaded += RefreshValue;
            _toggle.onValueChanged.AddListener(SetValue);
        }

        private void RefreshValue()
        {
            _toggle.isOn = _gameSettingsSO.GetValue<bool>(_controlPath);
        }
    }
}