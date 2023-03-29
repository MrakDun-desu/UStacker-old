using System.Linq;
using TMPro;
using UnityEngine;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;

namespace UStacker.Multiplayer
{
    public class MultiplayerBoard : MonoBehaviour
    {
        [SerializeField] private Mediator _mediator;
        [SerializeField] private GameObject[] _gameSettingsDependencies;
        [SerializeField] private TMP_Text _playerNameLabel;

        public Mediator Mediator => _mediator;

        private Player _ownerPlayer;
        private Player OwnerPlayer
        {
            get => _ownerPlayer;
            set
            {
                _ownerPlayer = value;
                _playerNameLabel.text = _ownerPlayer.DisplayName;
            }
        }

        private IGameSettingsDependency[] _dependantComponents;

        private GameSettingsSO.SettingsContainer _gameSettings;
        private GameSettingsSO.SettingsContainer GameSettings
        {
            get => _gameSettings;
            set
            {
                _gameSettings = value;
                foreach (var dependency in _dependantComponents)
                    dependency.GameSettings = GameSettings;
            }
        }
        
        private void Awake()
        {
            _dependantComponents =
                _gameSettingsDependencies.SelectMany(obj => obj.GetComponents<IGameSettingsDependency>()).ToArray();
        }

        public void Initialize(Player ownerPlayer, GameSettingsSO.SettingsContainer settings)
        {
            GameSettings = settings;
            _playerNameLabel.rectTransform.sizeDelta = new Vector2(
                _gameSettings.BoardDimensions.BoardWidth,
                _playerNameLabel.rectTransform.sizeDelta.y);
            OwnerPlayer = ownerPlayer;
        }

        public void Deactivate()
        {
            Mediator.Clear();
            gameObject.SetActive(false);
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }
    }
}