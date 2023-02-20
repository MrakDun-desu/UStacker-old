using System.Collections.Generic;
using UnityEngine;
using UStacker.Gameplay.Communication;

namespace UStacker.Gameplay
{
    public class GameRecorder : MonoBehaviour
    {
        [SerializeField] private Mediator _mediator;

        public List<InputActionMessage> ActionList = new();
        public List<PiecePlacementInfo> PiecePlacementList = new();

        private bool _recording;

        private void AddInputActionToList(InputActionMessage message)
        {
            ActionList.Add(message);
        }

        private void AddPiecePlacementToList(PiecePlacedMessage message)
        {
            var placementInfo = new PiecePlacementInfo(message.Time, message.TotalRotation, message.TotalMovement, message.PieceType);
            PiecePlacementList.Add(placementInfo);
        }

        public void StartRecording()
        {
            ActionList.Clear();
            PiecePlacementList.Clear();
            if (_recording) return;
            _mediator.Register<InputActionMessage>(AddInputActionToList);
            _mediator.Register<PiecePlacedMessage>(AddPiecePlacementToList);
            _recording = true;
        }

        public void StopRecording()
        {
            if (!_recording) return;
            _mediator.Unregister<InputActionMessage>(AddInputActionToList);
            _mediator.Unregister<PiecePlacedMessage>(AddPiecePlacementToList);
            _recording = false;
        }
    }
}