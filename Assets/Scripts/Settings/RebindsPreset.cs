using UnityEngine;

namespace Blockstacker.Settings
{
    [CreateAssetMenu(fileName = "Rebinds preset", menuName = "Blockstacker/Inputs")]
    public class RebindsPreset : ScriptableObject
    {
        public string Name;
        [TextArea(10, 30)] public string Content;
    }
}