using System.IO;
using System.Threading.Tasks;
using Blockstacker.Common;
using Blockstacker.GlobalSettings.BlockSkins;
using UnityEngine.Events;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class SkinApplier : SettingApplierBase
    {
        public UnityEvent LoadingStarted;
        public UnityEvent LoadingFinished;

        protected override void OnSettingChanged()
        {
            LoadingStarted.Invoke();
            _ = ReloadAndInvoke();
        }

        private async Task ReloadAndInvoke()
        {
            var skinFolder = Path.Combine(CustomizationPaths.Skins, AppSettings.Customization.SkinFolder);
            await SkinLoader.Reload(skinFolder);
            LoadingFinished.Invoke();
        }
    }
}