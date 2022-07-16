using UnityEngine;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class VSyncApplier : SettingApplierBase
    {
        protected override void OnSettingChanged()
        {
            QualitySettings.vSyncCount = AppSettings.Video.UseVsync ? 1 : 0;
        }
    }
}