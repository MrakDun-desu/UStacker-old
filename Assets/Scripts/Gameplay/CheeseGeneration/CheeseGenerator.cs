using System.Collections.Generic;
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

        private void Awake()
        {
            if (_settings.Objective.CheeseGeneration != GameSettings.Enums.CheeseGeneration.None ||
                _settings.Objective.UseCustomCheeseScript)
                _board.LinesCleared += GenerateCheeseLayer;
            
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
            var missingCheese = _settings.Objective.MaxCheeseHeight - _board.CheeseHeight;
            
            for (var y = 0; y < missingCheese; y++)
            {
                var newCheeseLayer = new List<List<bool>> {new()};
                var hole = _random.Next((int) _board.Width);
                while (hole == _lastHole)
                    hole = _random.Next((int) _board.Width);

                _lastHole = hole;
                
                for (var x = 0; x < _board.Width; x++)
                    newCheeseLayer[0].Add(hole != x);
                
                _board.AddCheeseLines(newCheeseLayer);
            }
        }
    }
}