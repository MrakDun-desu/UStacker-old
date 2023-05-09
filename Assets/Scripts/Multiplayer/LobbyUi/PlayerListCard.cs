
/************************************
PlayerListCard.cs -- created by Marek Dančo (xdanco00)
*************************************/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UStacker.Multiplayer.LobbyUi
{
    public class PlayerListCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _gamesParticipationLabel;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Sprite _defaultBackground;
        [SerializeField] private Sprite _spectatorBackground;

        private Player _player;

        public void SetPlayer(Player player)
        {
            _player = player;
            SetName(_player.DisplayName);
            SetSpectator(_player.IsSpectating);
            SetParticipation();

            _player.DisplayNameChanged += SetName;
            _player.SpectateChanged += SetSpectator;
            _player.Disconnected += OnPlayerDisconnected;
            _player.GamesPlayedChanged += SetParticipation;
            _player.GamesWonChanged += SetParticipation;
        }

        private void OnPlayerDisconnected()
        {
            _player.DisplayNameChanged -= SetName;
            _player.SpectateChanged -= SetSpectator;
            _player.Disconnected -= OnPlayerDisconnected;
            _player.GamesPlayedChanged -= SetParticipation;
            _player.GamesWonChanged -= SetParticipation;
            _player = null;
        }

        private void SetSpectator(bool isSpectator)
        {
            _backgroundImage.sprite = isSpectator ? _spectatorBackground : _defaultBackground;
        }

        private void SetParticipation(uint _ = 0)
        {
            _gamesParticipationLabel.text = $"{_player.GamesWon}/{_player.GamesPlayed}";
        }

        private void SetName(string newName)
        {
            _nameLabel.text = newName;
        }
    }
}
/************************************
end PlayerListCard.cs
*************************************/
