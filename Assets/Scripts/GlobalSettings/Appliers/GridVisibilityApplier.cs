
/************************************
GridVisibilityApplier.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;

namespace UStacker.GlobalSettings.Appliers
{
    public class GridVisibilityApplier : SettingApplierBase
    {
        public static event Action<float> VisibilityChanged;

        public override void OnSettingChanged()
        {
            VisibilityChanged?.Invoke(AppSettings.Gameplay.GridVisibility);
        }
    }
}
/************************************
end GridVisibilityApplier.cs
*************************************/
