﻿using System;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class ScrollToChangeVolumeApplier : SettingApplierBase
    {
        public static event Action<bool> ScrollToChangeVolumeChanged;

        public override void OnSettingChanged()
        {
            ScrollToChangeVolumeChanged?.Invoke(AppSettings.Gameplay.ScrollToChangeVolume);
        }
    }
}