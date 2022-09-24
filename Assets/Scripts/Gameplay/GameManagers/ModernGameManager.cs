using Blockstacker.Gameplay.Communication;
using UnityEngine;

namespace Blockstacker.Gameplay.GameManagers
{
    public class ModernGameManager : MonoBehaviour, IGameManager
    {
        private int _currentCombo;
        private uint _currentLevel;
        private bool _isBackToBack;
        private int _linesToNextLevel;
        private MediatorSO _mediator;

        public void Initialize(uint startingLevel, MediatorSO mediator)
        {
            _mediator = mediator;
        }
    }
}