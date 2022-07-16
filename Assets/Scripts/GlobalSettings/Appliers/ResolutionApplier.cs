using Blockstacker.Common.Extensions;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class ResolutionApplier : SettingApplierBase
    {
        protected override void OnSettingChanged()
        {
            var newResolution = AppSettings.Video.Resolution;
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