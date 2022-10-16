using System;
using Blockstacker.GlobalSettings.Enums;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class FullscreenModeApplier : SettingApplierBase
    {
        protected override void OnSettingChanged()
        {
            var newMode = AppSettings.Video.FullscreenMode switch
            {
                FullscreenMode.Fullscreen => FullScreenMode.ExclusiveFullScreen,
                FullscreenMode.BorderlessWindowed => FullScreenMode.FullScreenWindow,
                FullscreenMode.Windowed => FullScreenMode.Windowed,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            if (Screen.fullScreenMode == newMode) return;
            Screen.fullScreenMode = newMode;
        }
    }
}