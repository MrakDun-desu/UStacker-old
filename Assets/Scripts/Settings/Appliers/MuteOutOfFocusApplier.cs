using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Blockstacker.Settings.Appliers
{
    public class MuteOutOfFocusApplier : SettingApplierBase
    {
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private string _fieldName;

        public override void OnSettingChanged()
        {
            void MuteAudio(bool hasFocus)
            {
                if (hasFocus) {
                    _mixer.SetFloat(_fieldName, AppSettings.Sound.MasterVolume);
                }
                else {
                    _mixer.SetFloat(_fieldName, -80f);
                }
            }

            if (AppSettings.Sound.MuteWhenOutOfFocus) {
                Application.focusChanged += MuteAudio;
            }
            else {
                Application.focusChanged -= MuteAudio;
            }
        }

    }
}