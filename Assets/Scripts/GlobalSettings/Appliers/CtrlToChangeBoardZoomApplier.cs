using System;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class CtrlToChangeBoardZoomApplier : SettingApplierBase
    {
        public static event Action<bool> CtrlToChangeBoardZoomChanged;

        protected override void OnSettingChanged()
        {
            CtrlToChangeBoardZoomChanged?.Invoke(AppSettings.Gameplay.CtrlScrollToChangeBoardZoom);
        }
    }
}