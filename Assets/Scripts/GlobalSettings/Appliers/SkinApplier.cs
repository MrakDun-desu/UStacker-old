using Blockstacker.Loaders;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class SkinApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            SkinLoader.Reload();
        }
    }
}