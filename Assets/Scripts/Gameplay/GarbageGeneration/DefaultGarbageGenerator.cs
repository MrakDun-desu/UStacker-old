using System;
using System.Collections.Generic;
using Blockstacker.Gameplay.Communication;
using Blockstacker.GameSettings.Enums;

namespace Blockstacker.Gameplay.GarbageGeneration
{
    public class DefaultGarbageGenerator : IGarbageGenerator
    {
        private Random _random;
        private int _lastHole = -1;
        private int _linesLeft;
        private readonly ReadonlyBoard _board;
        private readonly List<int> _holeSizes = new();

        public DefaultGarbageGenerator(ReadonlyBoard board, GameSettings.Enums.GarbageGeneration garbageGeneration)
        {
            if (garbageGeneration.HasFlag(GameSettings.Enums.GarbageGeneration.Singles))
                _holeSizes.Add(1);
            if (garbageGeneration.HasFlag(GameSettings.Enums.GarbageGeneration.Doubles))
                _holeSizes.Add(2);
            if (garbageGeneration.HasFlag(GameSettings.Enums.GarbageGeneration.Triples))
                _holeSizes.Add(3);
            if (garbageGeneration.HasFlag(GameSettings.Enums.GarbageGeneration.Quads))
                _holeSizes.Add(4);

            _board = board;
        }

        public void ResetState(int seed)
        {
            _random = new Random(seed);
            _lastHole = -1;
            _linesLeft = 0;
        }

        public void GenerateGarbage(uint amount, PiecePlacedMessage _)
        {
            if (_holeSizes.Count < 1) return;

            var newGarbageLayer = new List<List<bool>>();
            var addToLast = _linesLeft > 0;
            
            while (amount > 0u)
            {
                amount--;

                if (_linesLeft <= 0)
                {
                    if (newGarbageLayer.Count > 0)
                    {
                        _board.AddGarbageLayer(newGarbageLayer, addToLast);
                        newGarbageLayer = new List<List<bool>>();
                    }

                    _linesLeft = _holeSizes[_random.Next(_holeSizes.Count)];
                    int newHole;
                    do
                        newHole = _random.Next((int) _board.Width);
                    while (newHole == _lastHole);
                    
                    _lastHole = newHole;
                    addToLast = false;
                }
                
                newGarbageLayer.Add(new List<bool>());
                _linesLeft--;
                for (var x = 0; x < _board.Width; x++)
                    newGarbageLayer[^1].Add(_lastHole != x);
            }
            
            _board.AddGarbageLayer(newGarbageLayer, addToLast);
        }
    }
}