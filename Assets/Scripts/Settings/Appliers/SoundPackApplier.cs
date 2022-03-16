using Blockstacker.Loaders;

namespace Blockstacker.Settings.Appliers
{
    public class SoundPackApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            SoundPackLoader.Reload();
        }
    }
}