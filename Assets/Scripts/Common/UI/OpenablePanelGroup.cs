using System.Linq;
using UnityEngine;

namespace UStacker.Common.UI
{
    public class OpenablePanelGroup : MonoBehaviour
    {
        [SerializeField] private OpenablePanel[] _openablePanels;
        [SerializeField] private GameObject _closeOverlay;
        private OpenablePanel _currentlyOpen;

        public void OpenPanel(OpenablePanel openedPanel)
        {
            if (!_openablePanels.Contains(openedPanel)) return;

            if (_currentlyOpen == openedPanel) return;
            // ReSharper disable once Unity.NoNullPropagation
            _currentlyOpen?.ClosePanel();
            openedPanel.OpenPanel();
            _currentlyOpen = openedPanel;
            _closeOverlay.SetActive(true);
        }

        public void TogglePanel(OpenablePanel toggledPanel)
        {
            if (!_openablePanels.Contains(toggledPanel)) return;

            if (_currentlyOpen == toggledPanel)
                CloseCurrent();
            else
            {
                // ReSharper disable once Unity.NoNullPropagation
                _currentlyOpen?.ClosePanel();
                toggledPanel.OpenPanel();
                _currentlyOpen = toggledPanel;
                _closeOverlay.SetActive(true);
            }
        }

        public void CloseCurrent()
        {
            // ReSharper disable once Unity.NoNullPropagation
            _currentlyOpen?.ClosePanel();
            _currentlyOpen = null;
            _closeOverlay.SetActive(false);
        }
    }
}