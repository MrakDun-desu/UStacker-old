using System;
using System.Collections.Generic;
using System.Linq;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;
using UStacker.GlobalSettings;
using UStacker.GlobalSettings.StatCounting;
using UnityEngine;

namespace UStacker.Gameplay.Stats
{
    public class StatCounterManager : MonoBehaviour
    {
        [SerializeField] private Board _board;
        [SerializeField] private Canvas _statCountersCanvas;
        [SerializeField] private StatCounterDisplayer _displayerPrefab;
        [SerializeField] private MediatorSO _mediator;
        [SerializeField] private GameTimer _timer;
        [SerializeField] private GameStateManager _gameStateManager;
        [SerializeField] private StatContainer _stats = new();

        public ReadonlyStatContainer Stats;

        public string GameType { get; set; }

        private void Start()
        {
            Stats = new ReadonlyStatContainer(_stats);
            _mediator.Register<InputActionMessage>(OnInputAction, true);
            _mediator.Register<HoldUsedMessage>(OnHold, true);
            _mediator.Register<PiecePlacedMessage>(OnPiecePlaced, true);
            _mediator.Register<GameStartedMessage>(ResetStats, true);
            _mediator.Register<GameRestartedMessage>(ResetStats, true);
            _mediator.Register<ScoreChangedMessage>(OnScoreChanged, true);
            _mediator.Register<LevelChangedMessage>(OnLevelChanged, true);
            CreateStatCounters();
        }

        private void Update()
        {
            if (!_gameStateManager.GameRunningActively) return;

            _stats.LinesPerMinute = _stats.LinesCleared / _timer.CurrentTime;
            _stats.PiecesPerSecond = _stats.PiecesPlaced / _timer.CurrentTime;
            _stats.KeysPerPiece = (double) _stats.KeysPressed / _stats.PiecesPlaced;
        }

        private void OnDestroy()
        {
            _mediator.Unregister<InputActionMessage>(OnInputAction);
            _mediator.Unregister<HoldUsedMessage>(OnHold);
            _mediator.Unregister<PiecePlacedMessage>(OnPiecePlaced);
            _mediator.Unregister<GameStartedMessage>(ResetStats);
            _mediator.Unregister<GameRestartedMessage>(ResetStats);
            _mediator.Unregister<ScoreChangedMessage>(OnScoreChanged);
            _mediator.Unregister<LevelChangedMessage>(OnLevelChanged);
        }

        private void ResetStats(object obj)
        {
            _stats.Reset();
        }

        private void CreateStatCounters()
        {
            var counterGroups = AppSettings.StatCounting.StatCounterGroups;
            counterGroups ??= new Dictionary<Guid, StatCounterGroup>();
            if (counterGroups.Count <= 0) return;

            AppSettings.StatCounting.GameStatCounterDictionary ??= new Dictionary<string, Guid>();

            var gameName = GameType;
            StatCounterGroup counterGroup = null;
            if (AppSettings.StatCounting.GameStatCounterDictionary.TryGetValue(gameName, out var groupId))
            {
                if (!AppSettings.StatCounting.StatCounterGroups.TryGetValue(groupId, out counterGroup)) return;
            }
            else
            {
                foreach (var (groupKey, group) in counterGroups)
                {
                    if (group.Name != gameName) continue;
                    AppSettings.StatCounting.GameStatCounterDictionary[gameName] = groupKey;
                    counterGroup = group;
                    break;
                }

                counterGroup ??= AppSettings.StatCounting.DefaultGroup;
            }

            var usedCounters = counterGroup.StatCounters.Where(counter => !string.IsNullOrEmpty(counter.Script))
                .ToArray();
            if (usedCounters.Length <= 0)
                return;

            var statUtility = new StatUtility(_timer);
            foreach (var statCounter in usedCounters)
            {
                var newCounter = Instantiate(_displayerPrefab, _statCountersCanvas.transform);

                newCounter.Initialize(_mediator, new StatBoardInterface(_board), Stats, statUtility, statCounter);
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