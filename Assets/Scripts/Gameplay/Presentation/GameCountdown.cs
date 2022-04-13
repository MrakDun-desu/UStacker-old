using TMPro;
using UnityEngine;

namespace Blockstacker.Gameplay.Presentation
{
    public class GameCountdown : MonoBehaviour
    {
        [SerializeField] TMP_Text _countDownText;

        public float CountdownInterval = .1f;
        public uint CountdownCount = 3;
    }
}