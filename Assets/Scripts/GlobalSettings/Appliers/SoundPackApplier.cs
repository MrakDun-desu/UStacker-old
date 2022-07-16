using System.IO;
using System.Threading.Tasks;
using Blockstacker.Common;
using Blockstacker.GlobalSettings.Music;
using UnityEngine.Events;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class SoundPackApplier : SettingApplierBase
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
            await SoundPackLoader.Reload(Path.Combine(CustomizationPaths.SoundPacks,
                AppSettings.Customization.SoundPackFolder));
            LoadingFinished.Invoke();
        }
    }
}