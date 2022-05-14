using System;

namespace Blockstacker.GlobalSettings.Appliers
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