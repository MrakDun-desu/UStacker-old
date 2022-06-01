using System;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class ColorGhostPieceApplier : SettingApplierBase
    {
        public static event Action<bool> ColorGhostPieceChanged;

        public override void OnSettingChanged()
        {
            ColorGhostPieceChanged?.Invoke(AppSettings.Gameplay.ColorGhostPiece);
        }
    }
}