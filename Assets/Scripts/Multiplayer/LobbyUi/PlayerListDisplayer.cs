
/************************************
PlayerListDisplayer.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace UStacker.Multiplayer.LobbyUi
{
    public class PlayerListDisplayer : MonoBehaviour
    {
        [SerializeField] private PlayerListCard _cardPrefab;
        [SerializeField] private RectTransform _cardsParent;
        [SerializeField] private TMP_Text _titleLabel;

        private readonly Dictionary<int, PlayerListCard> _playerListCards = new();
        private ObjectPool<PlayerListCard> _cardsPool;

        private void Awake()
        {
            Player.PlayerJoined += PlayerJoined;
            Player.PlayerLeft += PlayerLeft;
            _cardsPool = new ObjectPool<PlayerListCard>(
                () => Instantiate(_cardPrefab, _cardsParent),
                card => card.gameObject.SetActive(true),
                card => card.gameObject.SetActive(false),
                card => Destroy(card.gameObject));
        }

        private void OnDestroy()
        {
            Player.PlayerJoined -= PlayerJoined;
            Player.PlayerLeft -= PlayerLeft;
            _cardsPool.Dispose();
        }

        private void PlayerJoined(int playerId)
        {
            RefreshPlayerCount();

            if (_playerListCards.ContainsKey(playerId))
                return;

            var newCard = _cardsPool.Get();
            var newPlayer = Player.ConnectedPlayers[playerId];
            newPlayer.SpectateChanged += OnPlayerSpectateChanged;
            newCard.SetPlayer(newPlayer);

            _playerListCards[playerId] = newCard;
        }

        private void PlayerLeft(int playerId)
        {
            RefreshPlayerCount();
            Player.ConnectedPlayers[playerId].SpectateChanged -= OnPlayerSpectateChanged;

            if (!_playerListCards.ContainsKey(playerId))
                return;

            var playerCard = _playerListCards[playerId];
            _cardsPool.Release(playerCard);

            _playerListCards.Remove(playerId);
        }

        private void OnPlayerSpectateChanged(bool _)
        {
            RefreshPlayerCount();
        }

        private void RefreshPlayerCount()
        {
            var inactivePlayers = Player.ConnectedPlayers.Count(pair => pair.Value.IsSpectating);
            var activePlayers = Player.ConnectedPlayers.Count - inactivePlayers;

            _titleLabel.text = inactivePlayers == 0
                ? $"Players ({activePlayers})"
                : $"Players ({activePlayers}+{inactivePlayers})";
        }
    }
}
/************************************
end PlayerListDisplayer.cs
*************************************/
