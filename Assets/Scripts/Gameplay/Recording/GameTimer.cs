using UnityEngine;

namespace Blockstacker.Gameplay.Recording
{
    public class GameTimer : MonoBehaviour
    {
        public float CurrentTime { get; private set; }

        private void Update()
        {
            CurrentTime += Time.deltaTime;
        }
    }
}