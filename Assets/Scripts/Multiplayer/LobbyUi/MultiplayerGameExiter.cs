using FishNet;
using UnityEngine;
using UnityEngine.UI;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace UStacker.Multiplayer.LobbyUi
{
    public class MultiplayerGameExiter : MonoBehaviour
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private GameObject _confirmExitCanvas;
        [SerializeField] private Button _confirmExitButton;
        [SerializeField] private Button _cancelExitButton;

        private void Awake()
        {
            Player.LocalPlayerStarted += OnConnected;
        }

        private void OnDestroy()
        {
            Player.LocalPlayerStarted -= OnConnected;
        }

        private void OnConnected(Player localPlayer)
        {
            if (!InstanceFinder.IsHost)
            {
                _exitButton.onClick.AddListener(LoadMainMenu);
                return;
            }
            
            _exitButton.onClick.AddListener(OpenExitCanvas);
            _confirmExitButton.onClick.AddListener(LoadMainMenu);
            _cancelExitButton.onClick.AddListener(CloseExitCanvas);
        }

        private void OpenExitCanvas()
        {
            _confirmExitCanvas.SetActive(true);
            _cancelExitButton.Select();
        }

        private void CloseExitCanvas()
        {
            _confirmExitCanvas.SetActive(false);
        }

        private static void LoadMainMenu()
        {
            SceneManager.LoadScene("Scene_Menu_Main");
        }
    }
}