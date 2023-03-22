using System.Collections.Generic;
using UnityEngine;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;

namespace UStacker.Gameplay
{
    public class GameRecorder : MonoBehaviour
    {
        [SerializeField] private Mediator _mediator;

        public List<InputActionMessage> ActionList = new();
        public List<double> PiecePlacementTimes = new();

        private bool _recording;

        private void OnEnable()
        {
            _mediator.Register<GameStateChangedMessage>(OnGameStateChange);
        }

        private void OnDisable()
        {
            _mediator.Register<GameStateChangedMessage>(OnGameStateChange);
        }

        private void OnGameStateChange(GameStateChangedMessage message)
        {
            if (message.IsReplay)
                return;
            
            switch (message)
            {
                case {NewState: GameState.Initializing}:
                    StartRecording();
                    break;
                case {IsReplay: true}:
                    StopRecording();
                    break;
            }
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