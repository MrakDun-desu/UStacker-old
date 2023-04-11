using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.Pool;
using UStacker.Gameplay.Communication;
using UStacker.Multiplayer.Settings;

namespace UStacker.Multiplayer
{
    public class MultiplayerGameManager : NetworkBehaviour
    {
        [SerializeField] private MultiplayerBoardsOrganizer _boardsOrganizer;
        [SerializeField] private SettingsDependenciesFiller _dependenciesFiller;
        [SerializeField] private MultiplayerBoard _multiplayerBoardPrefab;
        
        private readonly Dictionary<int, MultiplayerBoard> _activeBoards = new();
        private readonly List<int> _readyPlayers = new();

        public event Action<MultiplayerGameState, MultiplayerGameState> GameStateChanged;

        private MultiplayerGameState _currentGameState = MultiplayerGameState.Off;

        public MultiplayerGameState CurrentGameState
        {
            get => _currentGameState;
            private set
            {
                GameStateChanged?.Invoke(_currentGameState, value);
                _currentGameState = value;
            }
        }

        private ObjectPool<MultiplayerBoard> _boardsPool;

        private void Awake()
        {
            _boardsPool = new ObjectPool<MultiplayerBoard>(CreateBoard, null, ReleaseBoard, DestroyBoard,
                true, 10, 2000); // max size is the same as maximum players
        }

        private void OnDestroy()
        {
            _boardsPool.Dispose();
        }

        #region Boards pool
        
        private MultiplayerBoard CreateBoard()
        {
            var newBoard = Instantiate(_multiplayerBoardPrefab, _boardsOrganizer.transform, true);
            return newBoard;
        }

        private void ReleaseBoard(MultiplayerBoard board)
        {
            _boardsOrganizer.RemoveBoard(board);
            board.gameObject.SetActive(false);
        }

        private void DestroyBoard(MultiplayerBoard board)
        {
            _boardsOrganizer.RemoveBoard(board);
            Destroy(board.gameObject);
        }


        #endregion
        
        #region Game state management

        public void ClearReadyPlayers()
        {
            _readyPlayers.Clear();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerAsReady(NetworkConnection sender = null)
        {
            if (sender is null)
                return;
            
            _readyPlayers.Add(sender.ClientId);
            var allPlayersReady = _activeBoards.Values.All(board => _readyPlayers.Contains(board.OwnerId));

            if (allPlayersReady)
                StartGame();
        }

        [ObserversRpc(RunLocally = true)]
        private void StartGame()
        {
            CurrentGameState = MultiplayerGameState.Running;
            foreach (var (id, board) in _activeBoards)
            {
                board.InitializeGame();
                if (id == Player.LocalPlayer.OwnerId)
                    board.StartGameCountdown();
            }
        }

        public void InitializeGame()
        {
            var gameSettings = MultiplayerSettingsManager.SettingsStatic.GameSettings;
            
            _dependenciesFiller.SetGameSettings(gameSettings);
            
            foreach (var player in Player.ActivePlayers)
            {
                var newBoard = _boardsPool.Get();
                var playerHandling = gameSettings.Controls.OverrideHandling ? null : player.Handling;
                newBoard.Initialize(player.OwnerId, gameSettings, playerHandling);
                newBoard.gameObject.SetActive(true);
                AddBoard(newBoard);
                if (Player.LocalPlayer == player)
                    _boardsOrganizer.SetMainBoard(newBoard);
                else
                    _boardsOrganizer.AddBoard(newBoard);
            }
            
            _boardsOrganizer.Reorganize();
            CurrentGameState = MultiplayerGameState.Initializing;
            StartCoroutine(SetReadyCor());
        }

        private IEnumerator SetReadyCor()
        {
            yield return new WaitForEndOfFrame();
            if (IsClient)
                SetPlayerAsReady();
        }

        [Server]
        [ContextMenu("End game manually")]
        private void EndGame()
        {
            foreach (var board in _activeBoards.Values)
            {
                
                _boardsPool.Release(board);
            }
            
            _activeBoards.Clear();
            CurrentGameState = MultiplayerGameState.Off;
        }
        
        private void AddBoard(MultiplayerBoard board)
        {
            var boardId = board.OwnerId;
            _activeBoards[boardId] = board;

            if (boardId != LocalConnection.ClientId) return;
        
            board.InputHandled += OnInputHandledClient;
            board.ProcessorUpdated += OnProcessorUpdatedClient;
        }

        #endregion
        
        #region Input and time sync

        private void OnProcessorUpdatedClient(double updateTime)
        {
            OnProcessorUpdated(updateTime);
        }

        [ServerRpc(RequireOwnership = false)]
        // ReSharper disable once UnusedParameter.Local
        private void OnProcessorUpdated(double updateTime, Channel channel = Channel.Unreliable, NetworkConnection sender = null)
        {
            if (sender is null)
                return;

            UpdateBoardTime(updateTime, sender.ClientId);
        }

        [ObserversRpc(RunLocally = true)]
        // ReSharper disable once UnusedParameter.Local
        private void UpdateBoardTime(double updateTime, int playerId, Channel channel = Channel.Unreliable)
        {
            if (playerId == LocalConnection.ClientId)
                return;

            var board = _activeBoards[playerId];
            if (!board.GameStarted)
                board.StartGameCountdown();
                
            board.UpdateTime(updateTime);
        }

        private void OnInputHandledClient(InputActionMessage message)
        {
            OnInputHandled(message);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnInputHandled(InputActionMessage message, NetworkConnection sender = null)
        {
            if (sender is null)
                return;
            
            HandleRemoteInput(message, sender.ClientId);
        }

        [ObserversRpc(RunLocally = true)]
        private void HandleRemoteInput(InputActionMessage message, int playerId)
        {
            if (!_activeBoards.TryGetValue(playerId, out var board))
                return;
            
            if (playerId == LocalConnection.ClientId)
            {
                // if the local board receives this message, it means that server got the input
                // message correctly and board doesn't need to wait for input anymore
                board.StopWaitingForInput(message);
                return;
            }
            
            if (!board.GameStarted)
                board.StartGameCountdown();
            
            board.HandleInput(message);
        }
        
        #endregion
    }
}