using System;
using System.Collections;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UStacker.Common.Alerts;
using UStacker.Multiplayer.Settings;
using Random = UnityEngine.Random;

namespace UStacker.Multiplayer
{
    public class MultiplayerLobbyManager : NetworkBehaviour
    {
        [SerializeField] private MultiplayerGameManager _gameManager;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private GameObject _lobbyCanvas;
        [SerializeField] private RectTransform _chatTransform;
        [SerializeField] private RectTransform _chatParent;
        [SerializeField] private RectTransform _chatIngameParent;
        [SerializeField] private GameObject _startGameCountdown;
        [SerializeField] private TMP_Text _countdownLabel;
        [Range(1, 10)] [SerializeField] private int _startGameCountdownCount = 3;

        private bool _matchStarting;

        private static MultiplayerLobbySettingsSo.SettingsContainer _lobbySettings =>
            MultiplayerSettingsManager.SettingsStatic.LobbySettings;

        private void Awake()
        {
            _gameManager.GameStateChanged += OnGameStateChanged;
        }

        private void OnDestroy()
        {
            _gameManager.GameStateChanged -= OnGameStateChanged;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            _startGameButton.gameObject.SetActive(Player.LocalPlayer.HasHostPrivileges);
            _startGameButton.onClick.AddListener(OnStartButtonClicked);
        }

        [Client]
        private void OnStartButtonClicked()
        {
            if (Player.LocalPlayer is not null && Player.LocalPlayer.HasHostPrivileges)
                OnStartButtonClickedServer();
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnStartButtonClickedServer(NetworkConnection sender = null)
        {
            if (sender is null || !Player.ConnectedPlayers[sender.ClientId].HasHostPrivileges)
                return; // don't need to show alert since the sender shouldn't even be trying this

            if (_gameManager.CurrentGameState != MultiplayerGameState.Off || _matchStarting)
            {
                GameStartDeclined(sender, "Game is already starting or in progress");
                return;
            }

            if (_lobbySettings.MinimumPlayers > Player.ActivePlayers.Count())
            {
                GameStartDeclined(sender, "Not enough players");
                return;
            }

            _matchStarting = true;

            var generalSettings = MultiplayerSettingsManager.SettingsStatic.GameSettings.General;
            if (!generalSettings.UseCustomSeed)
            {
                var seed1 = (ulong) ((long) Random.Range(int.MinValue, int.MaxValue) + int.MaxValue);
                var seed2 = (ulong) ((long) Random.Range(int.MinValue, int.MaxValue) + int.MaxValue);
                SetGameSeed(seed1 + (seed2 << 32));
            }

            _gameManager.ClearReadyPlayers();
            StartGameCountdown();
        }

        [ObserversRpc(RunLocally = true)]
        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void SetGameSeed(ulong newSeed)
        {
            MultiplayerSettingsManager.SettingsStatic.GameSettings.General.ActiveSeed = newSeed;
        }

        [TargetRpc]
        // ReSharper disable once MemberCanBeMadeStatic.Local
        // ReSharper disable once UnusedParameter.Local
        private void GameStartDeclined(NetworkConnection _, string reason)
        {
            AlertDisplayer.ShowAlert(new Alert("Couldn't start game!", reason, AlertType.Warning), false);
        }

        [ObserversRpc(RunLocally = true)]
        private void StartGameCountdown()
        {
            StartCoroutine(StartGameCor());
        }

        private IEnumerator StartGameCor()
        {
            _startGameButton.gameObject.SetActive(false);
            _startGameCountdown.SetActive(true);

            for (var countdownLeft = _startGameCountdownCount; countdownLeft >= 0; countdownLeft--)
            {
                _countdownLabel.text = countdownLeft > 0
                    ? $"Game starting in {countdownLeft}..."
                    : "Game starting!";

                yield return new WaitForSeconds(1);
            }

            _startGameCountdown.SetActive(false);
            
            _gameManager.InitializeGame();
        }

        private void OnGameStateChanged(MultiplayerGameState previousState, MultiplayerGameState newState)
        {
            switch (newState)
            {
                case MultiplayerGameState.Off:
                    _lobbyCanvas.SetActive(true);
                    _chatTransform.SetParent(_chatParent);
                    break;
                case MultiplayerGameState.Initializing:
                    _lobbyCanvas.SetActive(false);
                    _chatTransform.SetParent(_chatIngameParent);
                    _matchStarting = false;
                    break;
                case MultiplayerGameState.Running:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }
    }
}