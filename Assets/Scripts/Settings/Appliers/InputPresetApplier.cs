using UnityEngine;
using UnityEngine.InputSystem;

namespace Blockstacker.Settings.Appliers
{
    public class InputPresetApplier : SettingApplierBase
    {
        [SerializeField] private InputActionAsset _actionAsset;

        public override void OnSettingChanged()
        {
            _actionAsset.LoadBindingOverridesFromJson(AppSettings.Rebinds);
            Debug.Log(_actionAsset.SaveBindingOverridesAsJson());
        }
    }
}