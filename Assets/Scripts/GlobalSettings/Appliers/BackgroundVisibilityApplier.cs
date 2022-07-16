using System;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class BackgroundVisibilityApplier : SettingApplierBase
    {
        public static event Action<float> VisibilityChanged;

        protected override void OnSettingChanged()
        {
            VisibilityChanged?.Invoke(AppSettings.Video.BackgroundVisibility);
        }
    }
}