using Blockstacker.Loaders;

namespace Blockstacker.Settings.Appliers
{
    public class SkinApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            SkinLoader.Reload();
        }
    }
}