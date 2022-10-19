using System.IO;
using System.Threading.Tasks;
using Blockstacker.Common;
using Blockstacker.GlobalSettings.BlockSkins;
using UnityEngine;
using UnityEngine.Events;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class SkinApplier : SettingApplierBase, IAsyncApplier
    {
        [SerializeField] public UnityEvent _loadingStarted;
        [SerializeField] public UnityEvent _loadingFinished;

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
            var skinFolder = Path.Combine(CustomizationPaths.Skins, AppSettings.Customization.SkinFolder);
            await SkinLoader.ReloadAsync(skinFolder);
            LoadingFinished.Invoke();
        }
    }
}