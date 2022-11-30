using System;

namespace Blockstacker.GlobalSettings.Appliers
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