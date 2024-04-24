
/************************************
ResolutionApplier.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;
using UStacker.Common.Extensions;

namespace UStacker.GlobalSettings.Appliers
{
    public class ResolutionApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            var newResolution = AppSettings.Video.Resolution;
            if (newResolution.IsEqualTo(Screen.currentResolution)) return;

            Screen.SetResolution(
                newResolution.width,
                newResolution.height,
                Screen.fullScreenMode,
                newResolution.refreshRateRatio
            );

            if (float.IsInfinity(AppSettings.Video.TargetFramerate))
                QualitySettings.vSyncCount = 0;
            else
                Application.targetFrameRate = (int) AppSettings.Video.TargetFramerate;
        }
    }
}
/************************************
end ResolutionApplier.cs
*************************************/
