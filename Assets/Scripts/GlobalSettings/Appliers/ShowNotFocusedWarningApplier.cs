
/************************************
ShowNotFocusedWarningApplier.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;

namespace UStacker.GlobalSettings.Appliers
{
    public class ShowNotFocusedWarningApplier : SettingApplierBase
    {
        public static event Action<bool> ShowNotFocusedWarningChanged;

        public override void OnSettingChanged()
        {
            ShowNotFocusedWarningChanged?.Invoke(AppSettings.Gameplay.ShowNotFocusedWarning);
        }
    }
}
/************************************
end ShowNotFocusedWarningApplier.cs
*************************************/
