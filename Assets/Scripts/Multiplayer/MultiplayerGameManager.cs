using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;

namespace UStacker.Multiplayer
{
    public class MultiplayerGameManager : NetworkBehaviour
    {
        [SerializeField] private Button _startGameButton;

        private void Awake()
        {
            Player.LocalPlayerStarted += OnLocalPlayerStarted;
        }

        private void OnDestroy()
        {
            Player.LocalPlayerStarted -= OnLocalPlayerStarted;
        }

        private void OnLocalPlayerStarted(Player localPlayer)
        {
            _startGameButton.gameObject.SetActive(localPlayer.HasHostPrivileges);
        }
    }
}