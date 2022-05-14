using UnityEngine;
using UnityEngine.Serialization;

namespace Blockstacker.GameSettings
{
    [CreateAssetMenu(fileName = "Rotation", menuName = "Blockstacker/Rotation system")]
    public class RotationSystemSO : ScriptableObject
    {
        [FormerlySerializedAs("_rotationSystem")] public RotationSystem RotationSystem = new();
    }
}