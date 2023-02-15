using System.IO;
using System.Threading.Tasks;
using UStacker.GlobalSettings.BlockSkins;
using UnityEngine;
using UnityEngine.Events;
using UStacker.Common;

namespace UStacker.GlobalSettings.Appliers
{
    public class SkinApplier : SettingApplierBase, IAsyncApplier
    {
        [SerializeField] private bool _showAlert = true;
        public UnityEvent _loadingStarted;
        public UnityEvent _loadingFinished;

        public UnityEvent LoadingStarted => _loadingStarted;
        public UnityEvent LoadingFinished => _loadingFinished;
        public string OngoingMessage => "Skins loading...";

        public override void OnSettingChanged()
        {
            LoadingStarted.Invoke();
            _ = ReloadAndInvoke();
        }

        private async Task ReloadAndInvoke()
        {
            if (string.IsNullOrEmpty(AppSettings.Customization.SkinFolder))
                AppSettings.Customization.SkinFolder = SkinLoader.DEFAULT_PATH;
            
            var skinFolder = Path.Combine(PersistentPaths.Skins, AppSettings.Customization.SkinFolder);
            await SkinLoader.ReloadAsync(skinFolder, _showAlert);
            LoadingFinished.Invoke();
        }
    }
}