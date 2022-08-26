using System;
using Blockstacker.Gameplay;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Enums;
using UnityEngine;

namespace Gameplay.Stats
{
    public class StatCounter : MonoBehaviour
    {
        [SerializeField] private MediatorSO _mediator;
        [SerializeField] private GameTimer _timer;
        [SerializeField] private StatContainer _stats = new();

        public ReadonlyStatContainer Stats;

        private void Awake()
        {
            _mediator.Register<InputActionMessage>(OnInputAction);
            _mediator.Register<PiecePlacedMessage>(OnPiecePlaced);
            Stats = new ReadonlyStatContainer(_stats);
        }

        private void OnDestroy()
        {
            _mediator.Unregister<InputActionMessage>(OnInputAction);
            _mediator.Unregister<PiecePlacedMessage>(OnPiecePlaced);
        }

        private void OnInputAction(InputActionMessage message)
        {
            if (message.KeyActionType == KeyActionType.KeyDown) _stats.KeysPressed++;
            _stats.KeysPerSecond = _stats.KeysPressed / message.Time;
        }

        private void OnPiecePlaced(PiecePlacedMessage message)
        {
            _stats.PiecesPlaced++;
            _stats.LinesCleared += message.LinesCleared;
            _stats.GarbageLinesCleared += message.GarbageLinesCleared;

            if (message.WasAllClear) 
                _stats.AllClears++;
            if (message.CurrentCombo > _stats.LongestCombo) 
                _stats.LongestCombo = message.CurrentCombo;
            if (message.CurrentBackToBack > _stats.LongestBackToBack)
                _stats.LongestBackToBack = message.CurrentBackToBack;

            switch (message.LinesCleared)
            {
                case 0:
                    if (message.WasSpin)
                        _stats.Spins++;
                    else if (message.WasSpinMini)
                        _stats.MiniSpins++;
                    break;
                case 1:
                    if (message.WasSpin)
                        _stats.SpinSingles++;
                    else if (message.WasSpinMini)
                        _stats.MiniSpinSingles++;
                    else
                        _stats.Singles++;
                    break;
                case 2:
                    if (message.WasSpin)
                        _stats.SpinDoubles++;
                    else if (message.WasSpinMini)
                        _stats.MiniSpinDoubles++;
                    else
                        _stats.Doubles++;
                    break;
                case 3:
                    if (message.WasSpin)
                        _stats.SpinTriples++;
                    else if (message.WasSpinMini)
                        _stats.MiniSpinTriples++;
                    else
                        _stats.MiniSpinTriples++;
                    break;
                case 4:
                    if (message.WasSpin)
                        _stats.SpinQuads++;
                    else if (message.WasSpinMini)
                        _stats.MiniSpinQuads++;
                    else
                        _stats.Quads++;
                    break;
            }
        }

        private void Update()
        {
            _stats.LinesPerMinute = _stats.LinesCleared / _timer.CurrentTime;
            _stats.PiecesPerSecond = _stats.PiecesPlaced / _timer.CurrentTime;
            _stats.KeysPerPiece = (double)_stats.KeysPressed / _stats.PiecesPlaced;
        }

        public void ResetStats()
        {
            _stats.Reset();
        }
    }
}