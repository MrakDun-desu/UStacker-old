using System.Drawing;
using TMPro;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Blockstacker.Common
{
    public class TMP_ColorChanger : MonoBehaviour
    {
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        public void ChangeColor(string colorHex)
        {
            _text.color = CreateColor.FromString(colorHex);
        }
    }
}