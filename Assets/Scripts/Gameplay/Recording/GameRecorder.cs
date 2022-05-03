using System.Collections.Generic;
using UnityEngine;

namespace Blockstacker.Gameplay.Recording
{
    public class GameRecorder : MonoBehaviour
    {
        public List<IRecord> Records = new();
    }
}