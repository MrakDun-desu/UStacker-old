using UnityEngine;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class FullscreenModeApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            if (Screen.fullScreenMode == AppSettings.Video.FullscreenMode) return;
            Screen.fullScreenMode = AppSettings.Video.FullscreenMode;
        }
    }
}