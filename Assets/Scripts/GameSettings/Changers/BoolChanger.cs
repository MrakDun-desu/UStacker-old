using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.GameSettings.Changers
{
    public class BoolChanger : GameSettingChangerBase<bool>
    {
        [Space]
        [SerializeField] private Toggle _toggle;

        private void Start()
        {
            _toggle.isOn = _gameSettingsSO.GetValue<bool>(_controlPath);
        }
    }
}