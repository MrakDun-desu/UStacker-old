using System;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class CtrlToChangeBoardZoomApplier : SettingApplierBase
    {
        public static event Action<bool> CtrlToChangeBoardZoomChanged;
        public override void OnSettingChanged()
        {
            CtrlToChangeBoardZoomChanged?.Invoke(AppSettings.Gameplay.CtrlScrollToChangeBoardZoom);
        }
    }
}