using UnityEngine;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class FullscreenModeApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            var newMode = AppSettings.Video.FullscreenMode;
            
            if (Screen.fullScreenMode == newMode) return;
            Screen.fullScreenMode = newMode;
        }
    }
}