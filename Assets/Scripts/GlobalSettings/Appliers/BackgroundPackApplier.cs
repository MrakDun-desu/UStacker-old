using System.IO;
using System.Threading.Tasks;
using Blockstacker.Common;
using Blockstacker.GlobalSettings.Backgrounds;
using UnityEngine;
using UnityEngine.Events;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class BackgroundPackApplier : SettingApplierBase, IAsyncApplier
    {
        [SerializeField] public UnityEvent _loadingStarted;
        [SerializeField] public UnityEvent _loadingFinished;

        public UnityEvent LoadingStarted => _loadingStarted;
        public UnityEvent LoadingFinished => _loadingFinished;
        public string OngoingMessage => "Backgrounds loading...";

        public override void OnSettingChanged()
        {
            LoadingStarted.Invoke();
            _ = ReloadAndInvoke();
        }
        
        private async Task ReloadAndInvoke()
        {
            var backgroundFolder = Path.Combine(CustomizationPaths.BackgroundPacks,
                AppSettings.Customization.BackgroundFolder);
            await BackgroundPackLoader.Reload(backgroundFolder);
            LoadingFinished.Invoke();
        }
    }
}