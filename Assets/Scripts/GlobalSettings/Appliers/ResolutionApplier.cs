using Blockstacker.Common.Extensions;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class ResolutionApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            var newResolution = new Resolution
            {
                width = AppSettings.Video.Resolution.x,
                height = AppSettings.Video.Resolution.y,
                refreshRate = AppSettings.Video.RefreshRate
            };
            if (newResolution.IsEqualTo(Screen.currentResolution)) return;

            Screen.SetResolution(
                newResolution.width,
                newResolution.height,
                Screen.fullScreenMode,
                newResolution.refreshRate
            );

        }
    }
}