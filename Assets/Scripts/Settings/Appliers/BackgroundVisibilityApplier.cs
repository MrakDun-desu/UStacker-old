using System;

namespace Blockstacker.Settings.Appliers
{
    public class BackgroundVisibilityApplier : SettingApplierBase
    {
        public static event Action<float> VisibilityChanged;

        public override void OnSettingChanged()
        {
            VisibilityChanged?.Invoke(AppSettings.Video.BackgroundVisibility);
        }
    }
}