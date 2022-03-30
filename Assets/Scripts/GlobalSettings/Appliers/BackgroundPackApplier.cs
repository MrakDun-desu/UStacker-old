using Blockstacker.Loaders;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class BackgroundPackApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            BackgroundPackLoader.Reload();
        }
    }
}