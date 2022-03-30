using Blockstacker.Loaders;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class SoundPackApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            SoundPackLoader.Reload();
        }
    }
}