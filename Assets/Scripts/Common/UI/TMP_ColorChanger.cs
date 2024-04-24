
/************************************
TMP_ColorChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
using TMPro;
using UnityEngine;

namespace UStacker.Common.UI
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
/************************************
end TMP_ColorChanger.cs
*************************************/
