using UnityEngine;

namespace UStacker.GlobalSettings.Appliers
{
    public class NotFocusedWarning : MonoBehaviour
    {
        [SerializeField] private GameObject _warning;

        private void Awake()
        {
            ShowNotFocusedWarningApplier.ShowNotFocusedWarningChanged += OnSettingChanged;
            OnSettingChanged(AppSettings.Gameplay.ShowNotFocusedWarning);
        }

        private void OnSettingChanged(bool showOutOfFocusWarning)
        {
            if (showOutOfFocusWarning)
                Application.focusChanged += OnFocusChanged;
            else
                Application.focusChanged -= OnFocusChanged;
        }

        private void OnFocusChanged(bool hasFocus)
        {
            if (_warning == null) return;
            _warning.SetActive(!hasFocus);
        }
    }
}