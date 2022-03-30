using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Blockstacker.GlobalSettings.Appliers
{
    public class MuteOutOfFocusApplier : SettingApplierBase
    {
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private string _fieldName;

        protected override void Awake()
        {
            base.Awake();
            Muter.mixer = _mixer;
            Muter.fieldName = _fieldName;
        }

        public override void OnSettingChanged()
        {

            if (AppSettings.Sound.MuteWhenOutOfFocus) {
                Application.focusChanged += Muter.MuteAudio;
            }
            else {
                Application.focusChanged -= Muter.MuteAudio;
            }
        }

        internal static class Muter
        {
            public static AudioMixer mixer;
            public static string fieldName;
            public static void MuteAudio(bool hasFocus)
            {
                if (hasFocus) {
                    var volume = AppSettings.Sound.MasterVolume;
                    if (volume <= 0) volume = 0.0001f;
                    mixer.SetFloat(fieldName, Mathf.Log10(volume) * 20);
                }
                else {
                    mixer.SetFloat(fieldName, -80f);
                }
            }
        }

    }
}