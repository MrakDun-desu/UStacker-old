using System;
using System.Collections.Generic;
using UnityEngine;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.Initialization;
using UStacker.Gameplay.Stats;
using UStacker.GameSettings;
using UStacker.GlobalSettings;

namespace UStacker.Gameplay
{
    public class GameRecorder : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private Mediator _mediator;
        [SerializeField] private StatCounterManager _statCounterManager;

        private readonly List<InputActionMessage> ActionList = new();
        private readonly List<double> PiecePlacementTimes = new();
        public string GameType { get; set; }

        private GameSettingsSO.SettingsContainer _replaySettings;

        public GameSettingsSO.SettingsContainer GameSettings
        {
            // we need to copy the value here in case it gets changed during the game
            set => _replaySettings = value with { };
        }

        public GameReplay Replay { get; private set; }
        
        private bool _recording;

        private void OnEnable()
        {
            _mediator.Register<GameStateChangedMessage>(OnGameStateChange, 10);
            _mediator.Register<SeedSetMessage>(OnSeedSet);
        }

        private void OnDisable()
        {
            _mediator.Unregister<GameStateChangedMessage>(OnGameStateChange);
            _mediator.Unregister<SeedSetMessage>(OnSeedSet);
        }
        
        private void OnSeedSet(SeedSetMessage message)
        {
            _replaySettings.General.ActiveSeed = message.Seed;
        }

        private void OnGameStateChange(GameStateChangedMessage message)
        {
            switch (message)
            {
                case {IsReplay: true}:
                    StopRecording();
                    break;
                case {NewState: GameState.Initializing}:
                    StartRecording();
                    break;
                case {NewState: GameState.Ended}:
                    RecordReplay(message.Time);
                    break;
            }
        }
        private void RecordReplay(double endTime)
        {
            Replay ??= new GameReplay();
            Replay.GameType = GameType;
            Replay.GameSettings = _replaySettings;
            Replay.ActionList.Clear();
            Replay.PiecePlacementList.Clear();
            Replay.ActionList.AddRange(ActionList);
            Replay.PiecePlacementList.AddRange(PiecePlacementTimes);
            Replay.Stats = _statCounterManager.Stats;
            Replay.GameLength = endTime;
            Replay.TimeStamp = DateTime.UtcNow;

            if (AppSettings.Gameplay.AutosaveReplaysOnDisk)
                Replay.Save();
        }

        private void AddInputActionToList(InputActionMessage message)
        {
            ActionList.Add(message);
        }

        private void AddPiecePlacementToList(PiecePlacedMessage message)
        {
            PiecePlacementTimes.Add(message.Time);
        }

        private void StartRecording()
        {
            ActionList.Clear();
            PiecePlacementTimes.Clear();
            if (_recording) return;
            _mediator.Register<InputActionMessage>(AddInputActionToList);
            _mediator.Register<PiecePlacedMessage>(AddPiecePlacementToList);
            _recording = true;
        }

        private void StopRecording()
        {
            if (!_recording) return;
            _mediator.Unregister<InputActionMessage>(AddInputActionToList);
            _mediator.Unregister<PiecePlacedMessage>(AddPiecePlacementToList);
            _recording = false;
        }
    }
}