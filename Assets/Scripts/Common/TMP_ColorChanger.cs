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
            var systemColor = ColorTranslator.FromHtml(colorHex);

            _text.color = new Color(systemColor.R / 255f, systemColor.G / 255f, systemColor.B / 255f);
        }
    }
}