using System;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class FullscreenModeApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            if (!Enum.TryParse(AppSettings.Video.FullscreenMode, out FullScreenMode newMode))
                return;

            if (Screen.fullScreenMode == newMode) return;
            Screen.fullScreenMode = newMode;
        }
    }
}