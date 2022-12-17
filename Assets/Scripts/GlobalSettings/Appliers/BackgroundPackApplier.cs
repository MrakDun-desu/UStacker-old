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
        [SerializeField] private bool _showAlert = true;
        public UnityEvent _loadingStarted;
        public UnityEvent _loadingFinished;

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
            var backgroundFolder = Path.Combine(PersistentPaths.BackgroundPacks,
                AppSettings.Customization.BackgroundFolder);
            await BackgroundPackLoader.Reload(backgroundFolder, _showAlert);
            LoadingFinished.Invoke();
        }
    }
}