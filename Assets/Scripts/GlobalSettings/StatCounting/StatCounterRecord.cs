using System;
using UnityEngine;

namespace Blockstacker.GlobalSettings.StatCounting
{
    [Serializable]
    public class StatCounterRecord
    {
        public string Name;
        public bool IsLevellingSystem;
        public string Script;
        public Vector2 Position;
        public Vector2 Size;
        public float UpdateInterval;
    }
}