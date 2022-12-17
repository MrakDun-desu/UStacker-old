using System;
using System.Collections.Generic;
using Blockstacker.Gameplay.Communication;

namespace Blockstacker.Gameplay.GarbageGeneration
{
    public class DefaultGarbageGenerator : IGarbageGenerator
    {
        private readonly GarbageBoardInterface _boardInterface;
        private readonly List<int> _holeSizes = new();
        private int _lastHole = -1;
        private int _linesLeft;
        private Random _random;

        public DefaultGarbageGenerator(GarbageBoardInterface boardInterface, GameSettings.Enums.GarbageGeneration garbageGeneration)
        {
            if (garbageGeneration.HasFlag(GameSettings.Enums.GarbageGeneration.Singles))
                _holeSizes.Add(1);
            if (garbageGeneration.HasFlag(GameSettings.Enums.GarbageGeneration.Doubles))
                _holeSizes.Add(2);
            if (garbageGeneration.HasFlag(GameSettings.Enums.GarbageGeneration.Triples))
                _holeSizes.Add(3);
            if (garbageGeneration.HasFlag(GameSettings.Enums.GarbageGeneration.Quads))
                _holeSizes.Add(4);

            _boardInterface = boardInterface;
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
                        _boardInterface.AddGarbageLayer(newGarbageLayer, addToLast);
                        newGarbageLayer = new List<List<bool>>();
                    }

                    _linesLeft = _holeSizes[_random.Next(_holeSizes.Count)];

                    int newHole;
                    if (_lastHole == -1)
                        newHole = _random.Next((int) _boardInterface.Width);
                    else
                        newHole = (_lastHole + _random.Next((int) _boardInterface.Width - 1) + 1) % (int) _boardInterface.Width;

                    _lastHole = newHole;
                    addToLast = false;
                }

                newGarbageLayer.Add(new List<bool>());
                _linesLeft--;
                for (var x = 0; x < _boardInterface.Width; x++)
                    newGarbageLayer[^1].Add(_lastHole != x);
            }

            _boardInterface.AddGarbageLayer(newGarbageLayer, addToLast);
        }
    }
}