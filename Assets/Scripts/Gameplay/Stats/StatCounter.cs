using System;
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

        private void OnDestroy()
        {
            _mediator.Unregister<InputActionMessage>(OnInputAction);
            _mediator.Unregister<PiecePlacedMessage>(OnPiecePlaced);
        }

        private void OnInputAction(InputActionMessage midgameMessage)
        {
            if (midgameMessage.KeyActionType == KeyActionType.KeyDown) Stats.KeysPressed++;
        }

        private void OnPiecePlaced(PiecePlacedMessage midgameMessage)
        {
            Stats.PiecesPlaced++;
            Stats.LinesCleared += midgameMessage.LinesCleared;

            if (midgameMessage.WasAllClear) Stats.AllClears++;
            if (midgameMessage.CurrentCombo > Stats.LongestCombo) Stats.LongestCombo = midgameMessage.CurrentCombo;
            if (midgameMessage.CurrentBackToBack > Stats.LongestBackToBack)
                Stats.LongestBackToBack = midgameMessage.CurrentBackToBack;

            switch (midgameMessage.LinesCleared)
            {
                case 0:
                    if (midgameMessage.WasSpin)
                        Stats.Spins++;
                    else if (midgameMessage.WasSpinMini)
                        Stats.MiniSpins++;
                    break;
                case 1:
                    if (midgameMessage.WasSpin)
                        Stats.SpinSingles++;
                    else if (midgameMessage.WasSpinMini)
                        Stats.MiniSpinSingles++;
                    else
                        Stats.Singles++;
                    break;
                case 2:
                    if (midgameMessage.WasSpin)
                        Stats.SpinDoubles++;
                    else if (midgameMessage.WasSpinMini)
                        Stats.MiniSpinDoubles++;
                    else
                        Stats.Doubles++;
                    break;
                case 3:
                    if (midgameMessage.WasSpin)
                        Stats.SpinTriples++;
                    else if (midgameMessage.WasSpinMini)
                        Stats.MiniSpinTriples++;
                    else
                        Stats.MiniSpinTriples++;
                    break;
                case 4:
                    if (midgameMessage.WasSpin)
                        Stats.SpinQuads++;
                    else if (midgameMessage.WasSpinMini)
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