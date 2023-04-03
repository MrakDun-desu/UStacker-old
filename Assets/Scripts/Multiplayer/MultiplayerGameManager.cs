using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using UStacker.Gameplay.Communication;

namespace UStacker.Multiplayer
{
    public class MultiplayerGameManager : NetworkBehaviour
    {
        private readonly Dictionary<int, MultiplayerBoard> _playingBoards = new();
        
        public void AddBoard(MultiplayerBoard board)
        {
            var boardId = board.OwnerPlayer.OwnerId;
            _playingBoards[boardId] = board;
            
            if (boardId == LocalConnection.ClientId)
            {
                board.InputHandled += OnInputHandledClient;
                board.ProcessorUpdated += OnProcessorUpdatedClient;
            }
        }

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
            
            _playingBoards[playerId].UpdateTime(updateTime);
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
            if (playerId == LocalConnection.ClientId)
            {
                // if the local board receives this message, it means that server got the input
                // message correctly and board doesn't need to wait for input anymore
                _playingBoards[playerId].StopWaitingForInput(message);
                return;
            }
            
            _playingBoards[playerId].HandleInput(message);
        }
    }
}