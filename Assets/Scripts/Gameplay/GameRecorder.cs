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
            _mediator.Register<InputActionMessage>(AddActionToList);
        }

        private void AddActionToList(InputActionMessage message)
        {
            ActionList.Add(message);
            ActionList.Sort((a, b) => a.Time > b.Time ? 1 : -1);
        }

        public void ClearActionList()
        {
            ActionList.Clear();
        }
    }
}