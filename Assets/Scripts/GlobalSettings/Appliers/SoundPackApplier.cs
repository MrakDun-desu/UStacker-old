using System.IO;
using System.Threading.Tasks;
using UStacker.GlobalSettings.Music;
using UnityEngine;
using UnityEngine.Events;
using UStacker.Common;

namespace UStacker.GlobalSettings.Appliers
{
    public class SoundPackApplier : SettingApplierBase, IAsyncApplier
    {
        [SerializeField] private bool _showAlert = true;
        public UnityEvent _loadingStarted;
        public UnityEvent _loadingFinished;

        public UnityEvent LoadingStarted => _loadingStarted;
        public UnityEvent LoadingFinished => _loadingFinished;
        public string OngoingMessage => "Sound pack loading...";

        public override void OnSettingChanged()
        {
            LoadingStarted.Invoke();
            _ = ReloadAndInvoke();
        }

        private async Task ReloadAndInvoke()
        {
            if (string.IsNullOrEmpty(AppSettings.Customization.SoundPackFolder))
                AppSettings.Customization.SoundPackFolder = SoundPackLoader.DEFAULT_PATH;
            
            var soundPackFolder = Path.Combine(PersistentPaths.SoundPacks, AppSettings.Customization.SoundPackFolder);
            await SoundPackLoader.Reload(soundPackFolder, _showAlert);
            LoadingFinished.Invoke();
        }
    }
}