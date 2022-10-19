using System;

namespace Blockstacker.GlobalSettings.Appliers
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