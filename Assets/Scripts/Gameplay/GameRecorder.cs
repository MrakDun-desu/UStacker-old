
/************************************
GameRecorder.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
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

        private bool _recording;

        public string GameType
        {
            set => Replay.GameType = value;
        }

        public GameReplay Replay { get; set; } = new();

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

        public GameSettingsSO.SettingsContainer GameSettings
        {
            // we need to copy the value here in case it gets changed during the game
            set => Replay.GameSettings = value with { };
        }

        private void OnSeedSet(SeedSetMessage message)
        {
            Replay.GameSettings.General.ActiveSeed = message.Seed;
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
            Replay.Stats = _statCounterManager.Stats;
            Replay.GameLength = endTime;
            Replay.TimeStamp = DateTime.UtcNow;

            if (AppSettings.Gameplay.AutosaveReplaysOnDisk)
                Replay.Save();
        }

        private void AddInputActionToList(InputActionMessage message)
        {
            Replay.ActionList.Add(message);
        }

        private void AddPiecePlacementToList(PiecePlacedMessage message)
        {
            Replay.PiecePlacementList.Add(message.Time);
        }

        private void StartRecording()
        {
            Replay.ActionList.Clear();
            Replay.PiecePlacementList.Clear();
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
/************************************
end GameRecorder.cs
*************************************/
