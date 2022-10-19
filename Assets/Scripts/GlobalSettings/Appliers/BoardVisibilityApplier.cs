﻿using System;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class BoardVisibilityApplier : SettingApplierBase
    {
        public static event Action<float> VisibilityChanged;

        public override void OnSettingChanged()
        {
            VisibilityChanged?.Invoke(AppSettings.Gameplay.BoardVisibility);
        }
    }
}