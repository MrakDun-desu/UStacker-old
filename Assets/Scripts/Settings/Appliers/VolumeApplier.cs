using UnityEngine;
using UnityEngine.Audio;

namespace Blockstacker.Settings.Appliers
{
    public class VolumeApplier : SettingApplierBase
    {
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private string _propertyName;
        public override void OnSettingChanged()
        {
            string[] path = new string[] {
                "Sound",
                _propertyName
            };
            if (!AppSettings.SettingExists<float>(path)) {
                Debug.LogError("Setting could not be found!");
                return;
            }
            var value = AppSettings.GetValue<float>(path);
            if (value <= 0) value = 0.0001f;
            _mixer.SetFloat(_propertyName, Mathf.Log10(value) * 20);
        }
    }
}