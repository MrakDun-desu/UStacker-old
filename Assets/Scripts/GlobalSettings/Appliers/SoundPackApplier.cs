using Blockstacker.Music;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class SoundPackApplier : SettingApplierBase
    {
        [SerializeField] private SoundPackLoader _soundPackLoader;
        
        public override void OnSettingChanged()
        {
            _  = _soundPackLoader.Reload(AppSettings.Customization.SoundPackFolder);
        }
    }
}