using System;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class DragMouseToRepositionBoardApplier : SettingApplierBase
    {
        public static event Action<bool> DragMouseToRepositionBoardChanged;

        protected override void OnSettingChanged()
        {
            DragMouseToRepositionBoardChanged?.Invoke(AppSettings.Gameplay.DragMiddleButtonToRepositionBoard);
        }
    }
}