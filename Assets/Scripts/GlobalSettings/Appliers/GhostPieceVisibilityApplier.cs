
/************************************
GhostPieceVisibilityApplier.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;

namespace UStacker.GlobalSettings.Appliers
{
    public class GhostPieceVisibilityApplier : SettingApplierBase
    {
        public static event Action<float> VisibilityChanged;

        public override void OnSettingChanged()
        {
            VisibilityChanged?.Invoke(AppSettings.Gameplay.GhostPieceVisibility);
        }
    }
}
/************************************
end GhostPieceVisibilityApplier.cs
*************************************/
