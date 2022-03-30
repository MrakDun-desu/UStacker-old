using System;

namespace Blockstacker.GlobalSettings.Appliers
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