using UnityEngine;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;

namespace UStacker.Gameplay.GameStateManagement
{
    public class GameStateChangeEventReceiver : MonoBehaviour
    {
        [SerializeField] private GameStateChangeEvent[] _events;
        [SerializeField] private Mediator _mediator;

        private void Awake()
        {
            _mediator.Register<GameStateChangedMessage>(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameStateChangedMessage message)
        {
            foreach (var changeEvent in _events)
            {
                if (message.IsReplay && !changeEvent.WorkInReplay || !message.IsReplay && !changeEvent.WorkInGame)
                    continue;

                var previousFits = changeEvent.PreviousState == GameState.Any ||
                                   changeEvent.PreviousState == message.PreviousState;
                var newFits = changeEvent.NewState == GameState.Any ||
                                  changeEvent.NewState == message.NewState;
                
                if (previousFits && newFits)
                    changeEvent.OnStateChanged.Invoke();
            }
        }
    }
}