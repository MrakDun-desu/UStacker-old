using UnityEngine;

namespace Blockstacker.Settings.Appliers
{
    public class VSyncApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            QualitySettings.vSyncCount = AppSettings.Video.UseVsync ? 1 : 0;
        }
    }
}