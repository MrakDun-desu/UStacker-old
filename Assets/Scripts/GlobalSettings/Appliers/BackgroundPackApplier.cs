
/************************************
BackgroundPackApplier.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UStacker.Common;
using UStacker.GlobalSettings.Backgrounds;

namespace UStacker.GlobalSettings.Appliers
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
            if (string.IsNullOrEmpty(AppSettings.Customization.BackgroundFolder))
                AppSettings.Customization.BackgroundFolder = BackgroundPackLoader.DEFAULT_PATH;

            var backgroundFolder = Path.Combine(PersistentPaths.BackgroundPacks,
                AppSettings.Customization.BackgroundFolder);
            await BackgroundPackLoader.Reload(backgroundFolder, _showAlert);
            LoadingFinished.Invoke();
        }
    }
}
/************************************
end BackgroundPackApplier.cs
*************************************/
