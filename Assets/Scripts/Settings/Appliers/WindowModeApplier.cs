using System;
using UnityEngine;

namespace Blockstacker.Settings.Appliers
{
    public class WindowModeApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            Enum.TryParse(AppSettings.Video.FullscreenMode, out FullScreenMode newMode);

            if (Screen.fullScreenMode == newMode) return;
            Screen.fullScreenMode = newMode;
        }
    }
}