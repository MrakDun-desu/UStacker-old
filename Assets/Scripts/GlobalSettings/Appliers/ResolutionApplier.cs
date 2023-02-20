using UStacker.Common.Extensions;
using UnityEngine;

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