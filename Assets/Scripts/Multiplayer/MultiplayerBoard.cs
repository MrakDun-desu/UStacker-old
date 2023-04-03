using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.GameStateManagement;
using UStacker.Gameplay.Initialization;
using UStacker.Gameplay.InputProcessing;
using UStacker.Gameplay.Timing;
using UStacker.GameSettings;

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
        private Player _ownerPlayer;

        public Player OwnerPlayer
        {
            get => _ownerPlayer;
            private set
            {
                _ownerPlayer = value;
                _playerNameLabel.text = _ownerPlayer.DisplayName;
            }
        }

        private IGameSettingsDependency[] _dependantComponents = Array.Empty<IGameSettingsDependency>();

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
            if (Player.LocalPlayer != OwnerPlayer || _waitingForInput)
                return;
            
            ProcessorUpdated?.Invoke(updateTime);
        }
        
        private void OnInputHandled(InputActionMessage message)
        {
            if (Player.LocalPlayer != OwnerPlayer)
                return;

            _lastSentInput = message;
            InputHandled?.Invoke(message);
        }

        public void Initialize(Player ownerPlayer, GameSettingsSO.SettingsContainer settings)
        {
            if (_dependantComponents.Length == 0)
                _dependantComponents = GetComponentsInChildren<IGameSettingsDependency>(true);

            GameSettings = settings;
            OwnerPlayer = ownerPlayer;

            _inputProcessor.OverrideHandling = !GameSettings.Controls.OverrideHandling ? OwnerPlayer.Handling : null;

            _stateManager.IsReplay = Player.LocalPlayer != OwnerPlayer;
            _lastSentInput = null;
            
            foreach (var dependency in _dependantComponents)
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
            StartCoroutine(ScheduleGameStart());
        }

        private IEnumerator ScheduleGameStart()
        {
            yield return new WaitForEndOfFrame();
            _stateManager.InitializeGame();
        }

        private void OnGameStateChanged(GameStateChangedMessage message)
        {
            if (message.NewState == GameState.Initializing)
                _mediator.Send(new SeedSetMessage(GameSettings.General.ActiveSeed));
        }

        public void SetDetailLevel(BoardDetailLevel detailLevel)
        {
            // TODO 
            switch (detailLevel)
            {
                case BoardDetailLevel.Basic:
                    break;
                case BoardDetailLevel.Medium:
                    break;
                case BoardDetailLevel.Full:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(detailLevel), detailLevel, null);
            }
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
            _inputProcessor.HandleInputAction(message);
        }

        public void UpdateTime(double time)
        {
            if (time > _timer.CurrentTime)
                _timer.SetTime(time);
        }

        public void RegisterMediatorMessage<TMessage>(Action<TMessage, int> action)
            where TMessage : IMessage
        {
            _mediator.Register<TMessage>(message => OnCustomMessageReceived(message, action));
        }

        private void OnCustomMessageReceived<TMessage>(TMessage message, Action<TMessage, int> action)
            where TMessage : IMessage
        {
            action.Invoke(message, _ownerPlayer.OwnerId);
        }
        
        
    }
}