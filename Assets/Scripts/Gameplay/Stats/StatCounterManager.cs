using System.Linq;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Enums;
using Blockstacker.GameSettings;
using Blockstacker.GlobalSettings;
using Blockstacker.GlobalSettings.StatCounting;
using UnityEngine;

namespace Blockstacker.Gameplay.Stats
{
    public class StatCounterManager : MonoBehaviour
    {
        [SerializeField] private Board _board;
        [SerializeField] private Canvas _statCountersCanvas;
        [SerializeField] private StatCounterDisplayer _displayerPrefab;
        [SerializeField] private GameSettingsSO _gameSettings;
        [SerializeField] private MediatorSO _mediator;
        [SerializeField] private GameTimer _timer;
        [SerializeField] private StatContainer _stats = new();

        public ReadonlyStatContainer Stats;
        private bool _gameRunning;

        private void Start()
        {
            Stats = new ReadonlyStatContainer(_stats);
            _mediator.Register<InputActionMessage>(OnInputAction, true);
            _mediator.Register<PiecePlacedMessage>(OnPiecePlaced, true);
            _mediator.Register<GameStartedMessage>(OnGameStarted, true);
            _mediator.Register<GameEndedMessage>(OnGameEnded, true);
            _mediator.Register<GameRestartedMessage>(OnGameRestarted, true);
            CreateStatCounters();
        }

        private void CreateStatCounters()
        {
            var statUtility = new StatUtility(_timer);
            var gameName = _gameSettings.GameType.Value;
            StatCounterGroup counterGroup;
            if (AppSettings.StatCounting.GameStatCounterDictionary.TryGetValue(gameName, out var groupId))
            {
                if (!AppSettings.StatCounting.StatCounterGroups.TryGetValue(groupId, out counterGroup)) return;
            }
            else
            {
                var (groupKey, group) = AppSettings.StatCounting.StatCounterGroups.First();
                AppSettings.StatCounting.GameStatCounterDictionary[gameName] = groupKey;
                counterGroup = group;
            }


            foreach (var statCounter in counterGroup.StatCounters.Where(counter => !string.IsNullOrEmpty(counter.Script)))
            {
                var newCounter = Instantiate(_displayerPrefab, _statCountersCanvas.transform);
                
                newCounter.SetRequiredFields(_mediator, new StatBoardInterface(_board), Stats, statUtility, statCounter);
            }
        }

        private void OnDestroy()
        {
            _mediator.Unregister<InputActionMessage>(OnInputAction);
            _mediator.Unregister<PiecePlacedMessage>(OnPiecePlaced);
            _mediator.Unregister<GameStartedMessage>(OnGameStarted);
            _mediator.Unregister<GameEndedMessage>(OnGameEnded);
            _mediator.Unregister<GameRestartedMessage>(OnGameRestarted);
        }

        private void OnInputAction(InputActionMessage message)
        {
            if (message.KeyActionType == KeyActionType.KeyDown) _stats.KeysPressed++;
            _stats.KeysPerSecond = _stats.KeysPressed / message.Time;
        }

        private void OnGameRestarted(GameRestartedMessage _)
        {
            _gameRunning = false;
            _stats.Reset();
        }

        private void OnGameStarted(GameStartedMessage _)
        {
            _gameRunning = true;
        }

        private void OnGameEnded(GameEndedMessage _)
        {
            _gameRunning = false;
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

        private void Update()
        {
            if (!_gameRunning) return;
            
            _stats.LinesPerMinute = _stats.LinesCleared / _timer.CurrentTime;
            _stats.PiecesPerSecond = _stats.PiecesPlaced / _timer.CurrentTime;
            _stats.KeysPerPiece = (double)_stats.KeysPressed / _stats.PiecesPlaced;
        }
    }
}