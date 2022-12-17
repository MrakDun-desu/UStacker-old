using System;

namespace UStacker.GlobalSettings.Appliers
{
    public class DragMouseToRepositionBoardApplier : SettingApplierBase
    {
        public static event Action<bool> DragMouseToRepositionBoardChanged;

        public override void OnSettingChanged()
        {
            DragMouseToRepositionBoardChanged?.Invoke(AppSettings.Gameplay.DragMiddleButtonToRepositionBoard);
        }
    }
}