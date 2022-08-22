using System.Collections.Generic;
using Blockstacker.Common.Extensions;
using Blockstacker.Gameplay.Communication;
using Blockstacker.GameSettings;
using UnityEngine;

using Random = System.Random;

namespace Blockstacker.Gameplay.CheeseGeneration
{
    public class CheeseGenerator : MonoBehaviour
    {
        [SerializeField] private GameSettingsSO _settings;
        [SerializeField] private Board _board;
        [SerializeField] private MediatorSO _mediator;

        private Random _random;
        private int _lastHole = -1;
        private int _linesLeft;
        private List<int> _holeSizes;

        private void Awake()
        {
            if (_settings.Objective.CheeseGeneration != GameSettings.Enums.CheeseGeneration.None ||
                _settings.Objective.UseCustomCheeseScript)
                _board.LinesCleared += GenerateCheeseLayer;

            _holeSizes = new List<int>();
            var cheeseGeneration = _settings.Objective.CheeseGeneration;
            if (cheeseGeneration.HasFlag(GameSettings.Enums.CheeseGeneration.Singles))
                _holeSizes.Add(1);
            if (cheeseGeneration.HasFlag(GameSettings.Enums.CheeseGeneration.Doubles))
                _holeSizes.Add(2);
            if (cheeseGeneration.HasFlag(GameSettings.Enums.CheeseGeneration.Triples))
                _holeSizes.Add(3);
            if (cheeseGeneration.HasFlag(GameSettings.Enums.CheeseGeneration.Quads))
                _holeSizes.Add(4);
            
            _mediator.Register<GameStartedMessage>(StartGeneration);
        }

        private void StartGeneration(GameStartedMessage _)
        {
            _random = new Random(_settings.Rules.General.ActiveSeed);
            GenerateCheeseLayer();
        }

        private void OnDestroy()
        {
            _board.LinesCleared -= GenerateCheeseLayer;
            _mediator.Unregister<GameStartedMessage>(StartGeneration);
        }

        private void GenerateCheeseLayer()
        {
            if (_holeSizes.Count < 1) return;
            
            var missingCheese = _settings.Objective.MaxCheeseHeight - _board.GarbageHeight;
            
            var cheeseLayer = new List<List<bool>>();
            var addToLast = _linesLeft > 0;
            
            while (missingCheese > 0u)
            {
                missingCheese--;

                if (_linesLeft <= 0)
                {
                    if (cheeseLayer.Count > 0)
                    {
                        _board.AddCheeseLayer(cheeseLayer, addToLast);
                        cheeseLayer = new List<List<bool>>();
                    }

                    _linesLeft = _holeSizes[_random.Next(_holeSizes.Count)];
                    var newHole = _random.Next((int) _board.Width);
                    while (newHole == _lastHole)
                        newHole = _random.Next((int) _board.Width);
                    
                    _lastHole = newHole;
                    addToLast = false;
                }
                
                cheeseLayer.Add(new List<bool>());
                _linesLeft--;
                for (var x = 0; x < _board.Width; x++)
                    cheeseLayer[^1].Add(_lastHole != x);
            }
            
            _board.AddCheeseLayer(cheeseLayer, addToLast);
        }
    }
}