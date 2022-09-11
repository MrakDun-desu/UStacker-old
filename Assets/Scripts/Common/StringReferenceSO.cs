using UnityEngine;

namespace Blockstacker.Common
{
    [CreateAssetMenu(fileName = "StringReference", menuName = "Blockstacker/String reference")]
    public class StringReferenceSO : ScriptableObject
    {
        public string Value;
    }
}