using System.Collections.Generic;
using Blockstacker.Gameplay.Communication;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class GameRecorder : MonoBehaviour
    {
        [SerializeField] private MediatorSO _mediator;

        public List<InputActionMessage> ActionList = new();

        private void Awake()
        {
            _mediator.Register<InputActionMessage>(AddMessageToList);
        }

        private void AddMessageToList(InputActionMessage message)
        {
            ActionList.Add(message);
        }

        public void ClearActionList()
        {
            ActionList.Clear();
        }
    }
}