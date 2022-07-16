using System;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class ColorGhostPieceApplier : SettingApplierBase
    {
        public static event Action<bool> ColorGhostPieceChanged;

        protected override void OnSettingChanged()
        {
            ColorGhostPieceChanged?.Invoke(AppSettings.Gameplay.ColorGhostPiece);
        }
    }
}