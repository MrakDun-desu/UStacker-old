using UnityEngine;

namespace UStacker.Common
{
    [CreateAssetMenu(fileName = "StringReference", menuName = "UStacker/String reference")]
    public class StringReferenceSO : ScriptableObject
    {
        public string Value;
    }
}