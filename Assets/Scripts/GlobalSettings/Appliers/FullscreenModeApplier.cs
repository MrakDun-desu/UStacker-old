
/************************************
FullscreenModeApplier.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;
using UStacker.Common.Extensions;

namespace UStacker.GlobalSettings.Appliers
{
    public class FullscreenModeApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            if (Screen.fullScreenMode == AppSettings.Video.FullscreenMode) return;

            Screen.fullScreenMode = AppSettings.Video.FullscreenMode;

            if (AppSettings.Video.FullscreenMode == FullScreenMode.Windowed)
                return;

            // setting resolution here because if the former mode was windowed and window was resized,
            // it also changed the application's resolution. 
            var newResolution = AppSettings.Video.Resolution;
            if (newResolution.IsEqualTo(Screen.currentResolution)) return;

            Screen.SetResolution(
                newResolution.width,
                newResolution.height,
                Screen.fullScreenMode,
                newResolution.refreshRateRatio
            );
        }
    }
}
/************************************
end FullscreenModeApplier.cs
*************************************/
