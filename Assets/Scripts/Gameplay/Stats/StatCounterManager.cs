
/************************************
StatCounterManager.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.Initialization;
using UStacker.Gameplay.Timing;
using UStacker.GameSettings;
using UStacker.GlobalSettings;
using UStacker.GlobalSettings.StatCounting;

namespace UStacker.Gameplay.Stats
{
    public class StatCounterManager : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private Board _board;
        [SerializeField] private Canvas _statCountersCanvas;
        [SerializeField] private StatCounterDisplayer _displayerPrefab;
        [SerializeField] private GameTimer _timer;
        [SerializeField] private Mediator _mediator;
        [SerializeField] private StatContainer _stats = new();

        public ReadonlyStatContainer Stats;
        private readonly List<StatCounterDisplayer> _activeDisplayers = new();
        private bool _awake;
        private ObjectPool<StatCounterDisplayer> _displayerPool;

        private GameSettingsSO.SettingsContainer _gameSettings;

        private StatUtility _statUtility;

        private void Awake()
        {
            if (_awake)
                return;

            _awake = true;
            _statUtility = new StatUtility(_timer);
            Stats = new ReadonlyStatContainer(_stats);

            _displayerPool = new ObjectPool<StatCounterDisplayer>(
                CreateDisplayer,
                displayer => displayer.gameObject.SetActive(true),
                displayer => displayer.gameObject.SetActive(false),
                displayer => Destroy(displayer.gameObject)
            );
        }

        private void Update()
        {
            if (_timer.CurrentTime > 0)
            {
                _stats.LinesPerMinute = _stats.LinesCleared / _timer.CurrentTime * 60d;
                _stats.PiecesPerSecond = _stats.PiecesPlaced / _timer.CurrentTime;
            }

            if (_stats.PiecesPlaced > 0)
                _stats.KeysPerPiece = (double) _stats.KeysPressed / _stats.PiecesPlaced;
        }

        private void OnEnable()
        {
            _mediator.Register<InputActionMessage>(OnInputAction, 10);
            _mediator.Register<HoldUsedMessage>(OnHold, 10);
            _mediator.Register<PiecePlacedMessage>(OnPiecePlaced, 10);
            _mediator.Register<ScoreChangedMessage>(OnScoreChanged, 10);
            _mediator.Register<LevelChangedMessage>(OnLevelChanged, 10);
            _mediator.Register<GameStateChangedMessage>(OnGameStateChange);
        }

        private void OnDisable()
        {
            _mediator.Unregister<InputActionMessage>(OnInputAction);
            _mediator.Unregister<HoldUsedMessage>(OnHold);
            _mediator.Unregister<PiecePlacedMessage>(OnPiecePlaced);
            _mediator.Unregister<ScoreChangedMessage>(OnScoreChanged);
            _mediator.Unregister<LevelChangedMessage>(OnLevelChanged);
            _mediator.Unregister<GameStateChangedMessage>(OnGameStateChange);
        }

        private void OnDestroy()
        {
            _displayerPool.Dispose();
        }

        public GameSettingsSO.SettingsContainer GameSettings
        {
            private get => _gameSettings;
            set
            {
                _gameSettings = value;
                Awake();
                Initialize();
            }
        }

        private StatCounterDisplayer CreateDisplayer()
        {
            var newDisplayer = Instantiate(_displayerPrefab, _statCountersCanvas.transform);
            newDisplayer.Initialize(_mediator, new StatBoardInterface(_board), Stats, _statUtility);
            return newDisplayer;
        }

        private void OnGameStateChange(GameStateChangedMessage message)
        {
            if (message.NewState != GameState.Initializing)
                return;

            _stats.Reset();
        }

        private void Initialize()
        {
            foreach (var displayer in _activeDisplayers)
                _displayerPool.Release(displayer);

            _activeDisplayers.Clear();

            var counterGroup = GameSettings.Presentation.DefaultStatCounterGroup;

            if (GameSettings.Presentation.StatCounterGroupOverrideId is { } overrideId)
            {
                AppSettings.StatCounting.StatCounterGroups ??= new Dictionary<Guid, StatCounterGroup>();

                if (AppSettings.StatCounting.StatCounterGroups.TryGetValue(overrideId, out var group))
                    counterGroup = group;
            }

            var usedCounters = counterGroup.StatCounters.Where(counter => !string.IsNullOrEmpty(counter.Script))
                .ToArray();
            if (usedCounters.Length <= 0)
                return;
            foreach (var statCounter in usedCounters)
            {
                var displayer = _displayerPool.Get();
                displayer.RefreshStatCounter(statCounter);
                _activeDisplayers.Add(displayer);
            }
        }

        private void OnInputAction(InputActionMessage message)
        {
            if (message.KeyActionType == KeyActionType.KeyDown) _stats.KeysPressed++;
            _stats.KeysPerSecond = _stats.KeysPressed / message.Time;
        }

        private void OnHold(HoldUsedMessage message)
        {
            if (!message.WasSuccessful) return;
            _stats.Holds++;
        }

        private void OnPiecePlaced(PiecePlacedMessage message)
        {
            _stats.PiecesPlaced++;
            _stats.LinesCleared += message.LinesCleared;
            _stats.GarbageLinesCleared += message.GarbageLinesCleared;

            if (message.WasAllClear)
                _stats.AllClears++;
            if (message.CurrentCombo > _stats.LongestCombo)
                _stats.LongestCombo = message.CurrentCombo;
            if (message.CurrentBackToBack > _stats.LongestBackToBack)
                _stats.LongestBackToBack = message.CurrentBackToBack;

            switch (message.LinesCleared)
            {
                case 0:
                    if (message.WasSpin)
                        _stats.Spins++;
                    else if (message.WasSpinMini)
                        _stats.MiniSpins++;
                    break;
                case 1:
                    if (message.WasSpin)
                        _stats.SpinSingles++;
                    else if (message.WasSpinMini)
                        _stats.MiniSpinSingles++;
                    else
                        _stats.Singles++;
                    break;
                case 2:
                    if (message.WasSpin)
                        _stats.SpinDoubles++;
                    else if (message.WasSpinMini)
                        _stats.MiniSpinDoubles++;
                    else
                        _stats.Doubles++;
                    break;
                case 3:
                    if (message.WasSpin)
                        _stats.SpinTriples++;
                    else if (message.WasSpinMini)
                        _stats.MiniSpinTriples++;
                    else
                        _stats.MiniSpinTriples++;
                    break;
                case 4:
                    if (message.WasSpin)
                        _stats.SpinQuads++;
                    else if (message.WasSpinMini)
                        _stats.MiniSpinQuads++;
                    else
                        _stats.Quads++;
                    break;
            }
        }

        private void OnScoreChanged(ScoreChangedMessage message)
        {
            _stats.Score = message.Score;
        }

        private void OnLevelChanged(LevelChangedMessage message)
        {
            _stats.Level = message.Level;
        }
    }
}
/************************************
end StatCounterManager.cs
*************************************/
