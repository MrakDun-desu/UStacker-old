using System.IO;
using System.Threading.Tasks;
using Blockstacker.Common;
using Blockstacker.GlobalSettings.Music;
using UnityEngine;
using UnityEngine.Events;

namespace Blockstacker.GlobalSettings.Appliers
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
            var soundPackFolder = Path.Combine(PersistentPaths.SoundPacks, AppSettings.Customization.SoundPackFolder);
            await SoundPackLoader.Reload(soundPackFolder, _showAlert);
            LoadingFinished.Invoke();
        }
    }
}