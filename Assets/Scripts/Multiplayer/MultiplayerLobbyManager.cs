using System.Collections;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using UStacker.Common.Alerts;
using UStacker.Multiplayer.Settings;

namespace UStacker.Multiplayer
{
    public class MultiplayerLobbyManager : NetworkBehaviour
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private MultiplayerBoard _multiplayerBoardPrefab;
        [SerializeField] private GameObject _lobbyCanvas;
        [SerializeField] private RectTransform _chatTransform;
        [SerializeField] private RectTransform _chatParent;
        [SerializeField] private RectTransform _chatIngameParent;
        [SerializeField] private MultiplayerBoardsOrganizer _boardsOrganizer;
        [SerializeField] private GameObject _startGameCountdown;
        [SerializeField] private TMP_Text _countdownLabel;
        [Range(0, 1)] [SerializeField] private float _startGameCountdownInterval = 0.7f;
        [Range(1, 10)] [SerializeField] private int _startGameCountdownCount = 3;

        private ObjectPool<MultiplayerBoard> _boardsPool;
        private Player _localPlayer;

        private static MultiplayerLobbySettingsSo.SettingsContainer _lobbySettings =>
            MultiplayerSettingsManager.SettingsStatic.LobbySettings;

        public static bool GameInProgress { get; private set; }

        private MultiplayerBoard CreateBoard()
        {
            var newBoard = Instantiate(_multiplayerBoardPrefab, _boardsOrganizer.transform, true);
            return newBoard;
        }

        private void GetBoard(MultiplayerBoard board)
        {
            board.Activate();
            _boardsOrganizer.AddBoard(board);
        }

        private void ReleaseBoard(MultiplayerBoard board)
        {
            board.Deactivate();
            _boardsOrganizer.RemoveBoard(board);
        }

        private void DestroyBoard(MultiplayerBoard board)
        {
            _boardsOrganizer.RemoveBoard(board);
            Destroy(board.gameObject);
        }

        private void Awake()
        {
            _boardsPool = new ObjectPool<MultiplayerBoard>(CreateBoard, GetBoard, ReleaseBoard, DestroyBoard,
                true, 10, 2000); // max size is the same as maximum players
        }

        private void OnDestroy()
        {
            _boardsPool.Dispose();
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

            if (GameInProgress)
            {
                GameStartDeclined(sender, "Game is already starting or in progress");
                return;
            }

            if (!MultiplayerSettingsManager.SettingsSynchronized)
            {
                GameStartDeclined(sender, "Game settings aren't synchronized");
                return;
            }

            if (_lobbySettings.MinimumPlayers > Player.ActivePlayers.Count())
            {
                GameStartDeclined(sender, "Not enough players");
                return;
            }

            GameInProgress = true;
            StartGameCountdown();
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
                    ? $"Game starting in {countdownLeft}"
                    : "Game starting!";

                yield return new WaitForSeconds(_startGameCountdownInterval);
            }

            _startGameCountdown.SetActive(false);
            StartGame();
        }

        private void StartGame()
        {
            _lobbyCanvas.SetActive(false);
            _chatTransform.SetParent(_chatIngameParent);

            var gameSettings = MultiplayerSettingsManager.SettingsStatic.GameSettings;
            _boardsOrganizer.BoardSize = new Vector2Int(
                (int)gameSettings.BoardDimensions.BoardWidth,
                (int)gameSettings.BoardDimensions.BoardHeight
            );
            foreach (var player in Player.ActivePlayers)
            {
                var newBoard = _boardsPool.Get();
                newBoard.Initialize(player, gameSettings);
            }
        }

        [ContextMenu("End game manually")]
        [Server]
        public void EndGame()
        {
            _lobbyCanvas.SetActive(true);
            _chatTransform.SetParent(_chatParent);

            GameInProgress = false;
        }
    }
}