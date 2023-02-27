using System;
using UStacker.Common;
using UStacker.Common.Extensions;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings.Enums;
using TMPro;
using UnityEngine;
using UStacker.Gameplay.GameStateManagement;

namespace UStacker.Gameplay.Stats
{
    public class GameResultDisplayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _mainStatTitle;
        [SerializeField] private TMP_Text _mainStatText;
        [SerializeField] private StringReferenceSO _replayGameType;
        [SerializeField] private GameStateManager _stateManager;
        [SerializeField] private ReplayController _replayController;

        [Space]
        [SerializeField] private GameResultStatDisplayer _scoreText;
        [SerializeField] private GameResultStatDisplayer _timeText;
        [SerializeField] private GameResultStatDisplayer _levelText;
        [SerializeField] private GameResultStatDisplayer _linesText;
        [SerializeField] private GameResultStatDisplayer _piecesPlacedText;
        [SerializeField] private GameResultStatDisplayer _keysPressedText;
        [SerializeField] private GameResultStatDisplayer _singlesText;
        [SerializeField] private GameResultStatDisplayer _doublesText;
        [SerializeField] private GameResultStatDisplayer _triplesText;
        [SerializeField] private GameResultStatDisplayer _quadsText;
        [SerializeField] private GameResultStatDisplayer _spinsText;
        [SerializeField] private GameResultStatDisplayer _miniSpinsText;
        [SerializeField] private GameResultStatDisplayer _spinSinglesText;
        [SerializeField] private GameResultStatDisplayer _spinDoublesText;
        [SerializeField] private GameResultStatDisplayer _spinTriplesText;
        [SerializeField] private GameResultStatDisplayer _spinQuadsText;
        [SerializeField] private GameResultStatDisplayer _miniSpinSinglesText;
        [SerializeField] private GameResultStatDisplayer _miniSpinDoublesText;
        [SerializeField] private GameResultStatDisplayer _miniSpinTriplesText;
        [SerializeField] private GameResultStatDisplayer _miniSpinQuadsText;
        [SerializeField] private GameResultStatDisplayer _longestComboText;
        [SerializeField] private GameResultStatDisplayer _longestBackToBackText;
        [SerializeField] private GameResultStatDisplayer _allClearsText;
        [SerializeField] private GameResultStatDisplayer _holdsText;
        [SerializeField] private GameResultStatDisplayer _garbageLinesClearedText;
        [SerializeField] private GameResultStatDisplayer _piecesPerSecondText;
        [SerializeField] private GameResultStatDisplayer _keysPerPieceText;
        [SerializeField] private GameResultStatDisplayer _keysPerSecondText;
        [SerializeField] private GameResultStatDisplayer _linesPerMinuteText;

        private GameReplay _displayedReplay;

        public GameReplay DisplayedReplay
        {
            get => _displayedReplay;
            set
            {
                _displayedReplay = value;

                var stats = _displayedReplay.Stats;
                _mainStatText.text = _displayedReplay.GameSettings.Objective.MainStat switch
                {
                    MainStat.Score => stats.Score.ToString(),
                    MainStat.Time => _displayedReplay.GameLength.FormatAsTime(),
                    MainStat.LinesCleared => stats.LinesCleared.ToString(),
                    MainStat.GarbageLinesCleared => stats.GarbageLinesCleared.ToString(),
                    MainStat.PiecesUsed => stats.PiecesPlaced.ToString(),
                    _ => throw new ArgumentOutOfRangeException()
                };

                _mainStatTitle.text = _displayedReplay.GameSettings.Objective.MainStat switch
                {
                    MainStat.Score => "Final score",
                    MainStat.Time => "Final time",
                    MainStat.LinesCleared => "Lines cleared",
                    MainStat.GarbageLinesCleared => "Garbage lines cleared",
                    MainStat.PiecesUsed => "Pieces used",
                    _ => throw new ArgumentOutOfRangeException()
                };

                _scoreText.DisplayStat("Score", stats.Score);
                _timeText.DisplayStat("Time", _displayedReplay.GameLength, true);
                _levelText.DisplayStat("Final level", stats.Level);
                _linesText.DisplayStat("Lines cleared", stats.LinesCleared);
                _piecesPlacedText.DisplayStat("Pieces placed", stats.PiecesPlaced);
                _keysPressedText.DisplayStat("Keys pressed", stats.KeysPressed);
                _singlesText.DisplayStat("Singles", stats.Singles);
                _doublesText.DisplayStat("Doubles", stats.Doubles);
                _triplesText.DisplayStat("Triples", stats.Triples);
                _quadsText.DisplayStat("Quads", stats.Quads);
                _spinsText.DisplayStat("Spins", stats.Spins);
                _miniSpinsText.DisplayStat("Mini spins", stats.MiniSpins);
                _spinSinglesText.DisplayStat("Spin singles", stats.SpinSingles);
                _spinDoublesText.DisplayStat("Spin doubles", stats.SpinDoubles);
                _spinTriplesText.DisplayStat("Spin triples", stats.SpinTriples);
                _spinQuadsText.DisplayStat("Spin quads", stats.SpinQuads);
                _miniSpinSinglesText.DisplayStat("Mini spin singles", stats.MiniSpinSingles);
                _miniSpinDoublesText.DisplayStat("Mini spin doubles", stats.MiniSpinDoubles);
                _miniSpinTriplesText.DisplayStat("Mini spin triples", stats.MiniSpinTriples);
                _miniSpinQuadsText.DisplayStat("Mini spin quads", stats.MiniSpinQuads);
                _longestComboText.DisplayStat("Longest combo", stats.LongestCombo);
                _longestBackToBackText.DisplayStat("Longest back to back", stats.LongestBackToBack);
                _allClearsText.DisplayStat("All clears", stats.AllClears);
                _holdsText.DisplayStat("Holds used", stats.Holds);
                _garbageLinesClearedText.DisplayStat("Garbage lines cleared", stats.GarbageLinesCleared);
                _piecesPerSecondText.DisplayStat("Pieces per second", stats.PiecesPerSecond);
                _keysPerPieceText.DisplayStat("Keys per piece", stats.KeysPerPiece);
                _keysPerSecondText.DisplayStat("Keys per second", stats.KeysPerSecond);
                _linesPerMinuteText.DisplayStat("Lines per minute", stats.LinesPerMinute);
            }
        }

        public void TakeReplayFromInitializer()
        {
            DisplayedReplay = GameInitializer.Replay;
        }

        public void ShowReplay()
        {
            GameInitializer.Replay = _displayedReplay;
            GameInitializer.GameType = _replayGameType.Value;
            GameStateManager.IsReplay = true;

            _replayController.SetReplay(DisplayedReplay);
            _stateManager.InitializeGame();
        }

        public void PlayGameAgain()
        {
            GameInitializer.Replay = null;
            GameInitializer.GameType = _displayedReplay.GameType ?? _replayGameType.Value;
            GameStateManager.IsReplay = false;
            _stateManager.InitializeGame();
        }
    }
}