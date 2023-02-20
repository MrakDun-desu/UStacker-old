using UnityEngine;

namespace UStacker.GlobalSettings.Appliers
{
    public class TargetFramerateApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            if (float.IsInfinity(AppSettings.Video.TargetFramerate))
                QualitySettings.vSyncCount = 0;
            else
                Application.targetFrameRate = (int) AppSettings.Video.TargetFramerate;
        }
    }
}