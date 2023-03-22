using UnityEngine;
using UnityEngine.UI;

namespace UStacker.GameSettings.Changers
{
    public class GameSettingBoolChanger : GameSettingChangerBase<bool>
    {
        [Space] [SerializeField] private Toggle _toggle;

        protected override void Start()
        {
            base.Start();
            _toggle.onValueChanged.AddListener(SetValue);
        }

        protected override void RefreshValue()
        {
            _toggle.isOn = _gameSettingsSO.GetValue<bool>(_controlPath);
        }
    }
}