
/************************************
FocusChecker.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;
using UnityEngine.Events;
using UStacker.GlobalSettings;

namespace UStacker.Gameplay.GameStateManagement
{
    public class FocusChecker : MonoBehaviour
    {
        public UnityEvent FocusLost;

        private void Awake()
        {
            AppSettings.Gameplay.PauseSingleplayerGamesOutOfFocusChanged += OnPauseSettingChange;
            OnPauseSettingChange(AppSettings.Gameplay.PauseSingleplayerGamesOutOfFocus);
        }

        private void OnDestroy()
        {
            AppSettings.Gameplay.PauseSingleplayerGamesOutOfFocusChanged -= OnPauseSettingChange;
            Application.focusChanged -= OnFocusChanged;
        }

        private void OnPauseSettingChange(bool pauseGameOnFocusLoss)
        {
            if (pauseGameOnFocusLoss)
                Application.focusChanged += OnFocusChanged;
            else
                Application.focusChanged -= OnFocusChanged;
        }

        private void OnFocusChanged(bool hasFocus)
        {
            if (!hasFocus)
                FocusLost.Invoke();
        }
    }
}
/************************************
end FocusChecker.cs
*************************************/
