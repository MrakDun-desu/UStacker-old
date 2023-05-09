
/************************************
BackgroundVisibilityApplier.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;

namespace UStacker.GlobalSettings.Appliers
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
/************************************
end BackgroundVisibilityApplier.cs
*************************************/
