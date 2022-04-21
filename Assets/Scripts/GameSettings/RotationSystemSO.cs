using UnityEngine;

namespace Blockstacker.GameSettings
{
    [CreateAssetMenu(fileName = "Rotation", menuName = "Blockstacker/Rotation system")]
    public class RotationSystemSO : ScriptableObject
    {
        public RotationSystem _rotationSystem = new();
    }
}