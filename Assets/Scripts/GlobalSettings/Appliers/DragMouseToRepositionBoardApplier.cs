
/************************************
DragMouseToRepositionBoardApplier.cs -- created by Marek Dančo (xdanco00)
*************************************/
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
/************************************
end DragMouseToRepositionBoardApplier.cs
*************************************/
