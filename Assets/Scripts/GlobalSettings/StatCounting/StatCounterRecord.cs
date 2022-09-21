using System;
using UnityEngine;

namespace Blockstacker.GlobalSettings.StatCounting
{
    [Serializable]
    public class StatCounterRecord
    {
        public StatCounterType Type;
        public string Name;
        public string Filename;
        [TextArea(5, 100)] public string Script;
        public Vector2 Position;
        public Vector2 Size;
        public float UpdateInterval;
    }
}