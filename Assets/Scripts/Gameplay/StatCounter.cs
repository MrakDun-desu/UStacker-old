using System;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Enums;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class StatCounter : MonoBehaviour
    {
        [SerializeField] private MediatorSO _mediator;
        [SerializeField] private StatContainer _statContainer;
        
        public uint LinesCleared
        {
            get => _statContainer.LinesCleared;
            private set => _statContainer.LinesCleared = value;
        }
        public uint PiecesPlaced
        {
            get => _statContainer.PiecesPlaced;
            private set => _statContainer.PiecesPlaced = value;
        }
        public uint KeysPressed
        {
            get => _statContainer.KeysPressed;
            private set => _statContainer.KeysPressed = value;
        }
        
        public uint Singles
        {
            get => _statContainer.Singles;
            private set => _statContainer.Singles = value;
        }
        public uint Doubles
        {
            get => _statContainer.Doubles;
            private set => _statContainer.Doubles = value;
        }
        public uint Triples
        {
            get => _statContainer.Triples;
            private set => _statContainer.Triples = value;
        }
        public uint Quads
        {
            get => _statContainer.Quads;
            private set => _statContainer.Quads = value;
        }

        private void Awake()
        {
            _mediator.Register<InputActionMessage>(OnInputAction);
            _mediator.Register<PiecePlacedMessage>(OnPiecePlaced);
        }

        private void OnInputAction(InputActionMessage message)
        {
            if (message.KeyActionType == KeyActionType.KeyDown)
            {
                KeysPressed++;
            }
        }

        private void OnPiecePlaced(PiecePlacedMessage message)
        {
            PiecesPlaced++;
            LinesCleared += message.LinesCleared;

            switch (message.LinesCleared)
            {
                case 1:
                    Singles++;
                    break;
                case 2:
                    Doubles++;
                    break;
                case 3:
                    Triples++;
                    break;
                case 4:
                    Quads++;
                    break;
            }
        }

        public void ResetStats()
        {
            _statContainer = new StatContainer();
        }
        
    }
}