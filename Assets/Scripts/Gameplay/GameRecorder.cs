using System.Collections.Generic;
using UnityEngine;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Initialization;

namespace UStacker.Gameplay
{
    public class GameRecorder : MonoBehaviour, IMediatorDependency
    {
        public Mediator Mediator { private get; set; }

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
            Mediator.Register<InputActionMessage>(AddInputActionToList);
            Mediator.Register<PiecePlacedMessage>(AddPiecePlacementToList);
            _recording = true;
        }

        public void StopRecording()
        {
            if (!_recording) return;
            _recording = false;
        }
    }
}