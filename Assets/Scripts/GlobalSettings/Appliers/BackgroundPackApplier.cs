using System.IO;
using Blockstacker.Common;
using Blockstacker.GlobalSettings.Loaders;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class BackgroundPackApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            var backgroundFolder = Path.Combine(CustomizationPaths.BackgroundPacks,
                AppSettings.Customization.BackgroundFolder);
            _ = BackgroundPackLoader.Reload(backgroundFolder);
        }
    }
}