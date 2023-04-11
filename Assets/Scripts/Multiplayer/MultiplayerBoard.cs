using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.GameStateManagement;
using UStacker.Gameplay.Initialization;
using UStacker.Gameplay.InputProcessing;
using UStacker.Gameplay.Presentation;
using UStacker.Gameplay.Timing;
using UStacker.GameSettings;
using UStacker.GlobalSettings.Groups;

namespace UStacker.Multiplayer
{
    public class MultiplayerBoard : MonoBehaviour
    {
        [SerializeField] private Mediator _mediator;
        [SerializeField] private TMP_Text _playerNameLabel;
        [SerializeField] private InputProcessor _inputProcessor;
        [SerializeField] private GameTimer _timer;
        [SerializeField] private GameStateManager _stateManager;

        public event Action<double> ProcessorUpdated;
        public event Action<InputActionMessage> InputHandled;

        private InputActionMessage? _lastSentInput;
        private bool _waitingForInput => _lastSentInput is not null;
        private int _ownerId;

        public int OwnerId
        {
            get => _ownerId;
            private set
            {
                _ownerId = value;
                _playerNameLabel.text = Player.ConnectedPlayers[_ownerId].DisplayName;
            }
        }
        
        public bool GameStarted { get; private set; }

        private IGameSettingsDependency[] _settingsDependencies = Array.Empty<IGameSettingsDependency>();
        private IDetailLevelDependency[] _detailLevelDependencies = Array.Empty<IDetailLevelDependency>();

        private GameSettingsSO.SettingsContainer GameSettings { get; set; }

        private void Awake()
        {
            _inputProcessor.ProcessorUpdated += OnProcessorUpdated;
            _inputProcessor.InputHandled += OnInputHandled;
        }

        private void OnDestroy()
        {
            _inputProcessor.ProcessorUpdated -= OnProcessorUpdated;
            _inputProcessor.InputHandled -= OnInputHandled;
        }

        private void OnProcessorUpdated(double updateTime)
        {
            if (Player.LocalPlayer.OwnerId != OwnerId || _waitingForInput)
                return;
            
            ProcessorUpdated?.Invoke(updateTime);
        }
        
        private void OnInputHandled(InputActionMessage message)
        {
            if (Player.LocalPlayer.OwnerId != OwnerId)
                return;

            _lastSentInput = message;
            InputHandled?.Invoke(message);
        }

        public void Initialize(int ownerId, GameSettingsSO.SettingsContainer settings, HandlingSettings handling)
        {
            // we have to do this here because it's called before awake
            if (_settingsDependencies.Length == 0)
                _settingsDependencies = GetComponentsInChildren<IGameSettingsDependency>(true);

            if (_detailLevelDependencies.Length == 0)
                _detailLevelDependencies = GetComponentsInChildren<IDetailLevelDependency>(true);

            GameStarted = false;
            GameSettings = settings;
            OwnerId = ownerId;
            _inputProcessor.OverrideHandling = handling;

            var isReplaying = Player.LocalPlayer.OwnerId != OwnerId;
            _stateManager.IsReplay = isReplaying;
            _inputProcessor.ActionList = isReplaying ? new List<InputActionMessage>() : null;
            _inputProcessor.PlacementsList = isReplaying ? new List<double>() : null;
            _lastSentInput = null;
            
            foreach (var dependency in _settingsDependencies)
                dependency.GameSettings = GameSettings;
        }

        private void OnDisable()
        {
            _mediator.Clear();
            GameStateChangeEventReceiver.Deactivate();
        }

        private void OnEnable()
        {
            GameStateChangeEventReceiver.Activate();
            _mediator.Register<GameStateChangedMessage>(OnGameStateChanged, 10);
        }

        public void InitializeGame()
        {
            _stateManager.InitializeGame(false);
        }

        public void StartGameCountdown()
        {
            _stateManager.StartCountdown();
            GameStarted = true;
        }

        private void OnGameStateChanged(GameStateChangedMessage message)
        {
            if (message.NewState == GameState.Initializing)
                _mediator.Send(new SeedSetMessage(GameSettings.General.ActiveSeed));
        }

        public void SetDetailLevel(BoardDetailLevel detailLevel)
        {
            foreach (var dependency in _detailLevelDependencies)
                dependency.DetailLevel = detailLevel;
        }

        public void StopWaitingForInput(InputActionMessage message)
        {
            if (_lastSentInput is not { } input)
                return;
            
            if (message.Equals(input))
                _lastSentInput = null;
        }

        public void HandleInput(InputActionMessage message)
        {
            _inputProcessor.AddActionToList(message);
        }

        public void UpdateTime(double time)
        {
            _timer.TweenTimeForward(time);
        }

        public void RegisterOnMediator<TMessage>(Action<TMessage, int> action)
            where TMessage : IMessage
        {
            void HandleMessage(TMessage message) => action.Invoke(message, _ownerId);

            _mediator.Register<TMessage>(HandleMessage);
        }
    }
}