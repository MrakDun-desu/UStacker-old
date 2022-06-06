using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Enums;
using UnityEngine;

namespace Gameplay.Stats
{
    public class StatCounter : MonoBehaviour
    {
        [SerializeField] private MediatorSO _mediator;
        [SerializeField] public StatContainer Stats = new();

        private void Awake()
        {
            _mediator.Register<InputActionMessage>(OnInputAction);
            _mediator.Register<PiecePlacedMessage>(OnPiecePlaced);
        }

        private void OnInputAction(InputActionMessage message)
        {
            if (message.KeyActionType == KeyActionType.KeyDown) Stats.KeysPressed++;
        }

        private void OnPiecePlaced(PiecePlacedMessage message)
        {
            Stats.PiecesPlaced++;
            Stats.LinesCleared += message.LinesCleared;

            switch (message.LinesCleared)
            {
                case 0:
                    if (message.WasSpin)
                        Stats.Spins++;
                    else if (message.WasSpinMini)
                        Stats.MiniSpins++;
                    break;
                case 1:
                    if (message.WasSpin)
                        Stats.SpinSingles++;
                    else if (message.WasSpinMini)
                        Stats.MiniSpinSingles++;
                    else
                        Stats.Singles++;
                    break;
                case 2:
                    if (message.WasSpin)
                        Stats.SpinDoubles++;
                    else if (message.WasSpinMini)
                        Stats.MiniSpinDoubles++;
                    else
                        Stats.Doubles++;
                    break;
                case 3:
                    if (message.WasSpin)
                        Stats.SpinTriples++;
                    else if (message.WasSpinMini)
                        Stats.MiniSpinTriples++;
                    else
                        Stats.MiniSpinTriples++;
                    break;
                case 4:
                    if (message.WasSpin)
                        Stats.SpinQuads++;
                    else if (message.WasSpinMini)
                        Stats.MiniSpinQuads++;
                    else
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