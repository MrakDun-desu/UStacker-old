using System.Collections.Generic;
using Blockstacker.Gameplay.Communication;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class GameRecorder : MonoBehaviour
    {
        [SerializeField] private MediatorSO _mediator;

        public List<InputActionMessage> ActionList = new();
        private bool _recording;

        private void AddMessageToList(InputActionMessage message)
        {
            ActionList.Add(message);
        }

        public void StartRecording()
        {
            ActionList.Clear();
            if (_recording) return;
            _mediator.Register<InputActionMessage>(AddMessageToList);
            _recording = true;
        }

        public void StopRecording()
        {
            if (!_recording) return;
            _mediator.Unregister<InputActionMessage>(AddMessageToList);
            _recording = false;
        }
    }
}