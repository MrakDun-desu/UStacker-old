using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.GameStateManagement;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;
using UStacker.Multiplayer.Enums;

namespace UStacker.Multiplayer
{
    public class MultiplayerBoard : MonoBehaviour
    {
        [SerializeField] private Mediator _mediator;
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

        private IGameSettingsDependency[] _dependantComponents = Array.Empty<IGameSettingsDependency>();

        private GameSettingsSO.SettingsContainer _gameSettings;
        private GameSettingsSO.SettingsContainer GameSettings
        {
            get => _gameSettings;
            set => _gameSettings = value;
        }

        private void Awake()
        {
            if (_dependantComponents.Length == 0)
                _dependantComponents = GetComponentsInChildren<IGameSettingsDependency>();
        }

        public void Initialize(Player ownerPlayer, GameSettingsSO.SettingsContainer settings)
        {
            GameSettings = settings;
            OwnerPlayer = ownerPlayer;
        }

        private void OnDisable()
        {
            _mediator.Clear();
            GameStateChangeEventReceiver.Deactivate();
        }

        private void OnEnable()
        {
            foreach (var dependency in _dependantComponents)
                dependency.GameSettings = GameSettings;
            GameStateChangeEventReceiver.Activate();
            _mediator.Register<GameStateChangedMessage>(OnGameStateChanged, 10);
        }

        private void OnGameStateChanged(GameStateChangedMessage message)
        {
            if (message.NewState == GameState.Initializing)
                _mediator.Send(new SeedSetMessage(GameSettings.General.ActiveSeed));
        }
    }
}