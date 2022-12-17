using UnityEngine;

namespace UStacker.GlobalSettings.Appliers
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