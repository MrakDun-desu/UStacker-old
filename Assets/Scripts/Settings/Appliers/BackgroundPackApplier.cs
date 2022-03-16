using Blockstacker.Loaders;

namespace Blockstacker.Settings.Appliers
{
    public class BackgroundPackApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            BackgroundPackLoader.Reload();
        }
    }
}