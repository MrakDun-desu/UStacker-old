
/************************************
WarningPieceTreshholdApplier.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;

namespace UStacker.GlobalSettings.Appliers
{
    public class WarningPieceTreshholdApplier : SettingApplierBase
    {
        public static event Action<float> TreshholdChanged;

        public override void OnSettingChanged()
        {
            TreshholdChanged?.Invoke(AppSettings.Gameplay.WarningPieceTreshhold);
        }
    }
}
/************************************
end WarningPieceTreshholdApplier.cs
*************************************/
