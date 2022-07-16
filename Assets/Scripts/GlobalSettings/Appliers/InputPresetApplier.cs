using UnityEngine;
using UnityEngine.InputSystem;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class InputPresetApplier : SettingApplierBase
    {
        [SerializeField] private InputActionAsset _actionAsset;

        protected override void OnSettingChanged()
        {
            _actionAsset.LoadBindingOverridesFromJson(AppSettings.Rebinds);
        }
    }
}