using UnityEngine;

namespace Blockstacker.GameSettings
{
    [CreateAssetMenu(fileName = "KickSystem", menuName = "Blockstacker/Kick system")]
    public class KickSystemSO : ScriptableObject
    {
        public KickSystem KickSystem = new();
    }
}