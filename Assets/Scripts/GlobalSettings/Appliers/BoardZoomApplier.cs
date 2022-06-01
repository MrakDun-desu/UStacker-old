using System;

namespace Blockstacker.GlobalSettings.Appliers
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