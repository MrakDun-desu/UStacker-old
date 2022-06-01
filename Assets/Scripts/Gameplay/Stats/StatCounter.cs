using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Enums;
using UnityEngine;

namespace Gameplay.Stats
{
    public class StatCounter : MonoBehaviour
    {
        [SerializeField] private MediatorSO _mediator;
        public readonly StatContainer Stats = new ();
        
        private void Awake()
        {
            _mediator.Register<InputActionMessage>(OnInputAction);
            _mediator.Register<PiecePlacedMessage>(OnPiecePlaced);
        }

        private void OnInputAction(InputActionMessage message)
        {
            if (message.KeyActionType == KeyActionType.KeyDown)
            {
                Stats.KeysPressed++;
            }
        }

        private void OnPiecePlaced(PiecePlacedMessage message)
        {
            Stats.PiecesPlaced++;
            Stats.LinesCleared += message.LinesCleared;

            switch (message.LinesCleared)
            {
                case 1:
                    Stats.Singles++;
                    break;
                case 2:
                    Stats.Doubles++;
                    break;
                case 3:
                    Stats.Triples++;
                    break;
                case 4:
                    Stats.Quads++;
                    break;
            }
        }

        public void ResetStats()
        {
            Stats.Reset();
        }
        
    }
}