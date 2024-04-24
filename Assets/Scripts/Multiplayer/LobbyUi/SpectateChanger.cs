
/************************************
SpectateChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UStacker.Multiplayer.LobbyUi
{
    public class SpectateChanger : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _buttonLabel;

        private Player _localPlayer;

        private void Awake()
        {
            _button.onClick.AddListener(ToggleSpectate);
            Player.LocalPlayerStarted += OnConnected;
        }

        private void OnDestroy()
        {
            Player.LocalPlayerStarted -= OnConnected;
        }

        private void OnDisconnected()
        {
            _localPlayer.Disconnected -= OnDisconnected;
            _localPlayer.SpectateChanged -= OnSpectateChanged;
            _localPlayer = null;
        }

        private void OnConnected(Player localPlayer)
        {
            _localPlayer = localPlayer;
            localPlayer.SpectateChanged += OnSpectateChanged;
            localPlayer.Disconnected += OnDisconnected;
            OnSpectateChanged(localPlayer.IsSpectating);
        }

        private void OnSpectateChanged(bool isSpectating)
        {
            _buttonLabel.text = isSpectating ? "Play" : "Spectate";
        }

        private void ToggleSpectate()
        {
            if (_localPlayer is null) return;

            _localPlayer.IsSpectating = !_localPlayer.IsSpectating;
        }
    }
}
/************************************
end SpectateChanger.cs
*************************************/
