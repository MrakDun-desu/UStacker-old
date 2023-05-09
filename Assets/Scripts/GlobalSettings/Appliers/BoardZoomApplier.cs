
/************************************
BoardZoomApplier.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;

namespace UStacker.GlobalSettings.Appliers
{
    public class BoardZoomApplier : SettingApplierBase
    {
        public static event Action<float> BoardZoomChanged;

        public override void OnSettingChanged()
        {
            BoardZoomChanged?.Invoke(AppSettings.Gameplay.BoardZoom);
        }
    }
}
/************************************
end BoardZoomApplier.cs
*************************************/
