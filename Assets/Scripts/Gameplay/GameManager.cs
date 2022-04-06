using Blockstacker.Gameplay.Pieces;
using Blockstacker.Gameplay.Randomizers;
using Blockstacker.GameSettings;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameSettingsSO _gameSettings;
        [SerializeField] private Board _board;

        public IRandomizer randomizer;

    }
}