using UnityEngine;
using UnityEngine.InputSystem;

namespace UStacker.GlobalSettings.Appliers
{
    public class InputPresetApplier : SettingApplierBase
    {
        [SerializeField] private InputActionAsset _actionAsset;

        public override void OnSettingChanged()
        {
            _actionAsset.LoadBindingOverridesFromJson(AppSettings.Rebinds);
        }
    }
}