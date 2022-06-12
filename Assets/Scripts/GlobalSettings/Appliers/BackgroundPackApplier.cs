using System.IO;
using Blockstacker.Loaders;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class BackgroundPackApplier : SettingApplierBase
    {
        public override void OnSettingChanged()
        {
            var backgroundFolder = Path.Combine(Application.persistentDataPath, "backgroundPacks",
                AppSettings.Customization.BackgroundFolder);
            _ = BackgroundPackLoader.Reload(backgroundFolder);
        }
    }
}