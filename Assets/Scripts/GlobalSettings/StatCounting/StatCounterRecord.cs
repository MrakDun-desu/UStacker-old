using System;
using UnityEngine;

namespace Blockstacker.GlobalSettings.StatCounting
{
    [Serializable]
    public class StatCounterRecord
    {
        public string Name;
        public StatCounterType Type;
        public string Filename;
        [TextArea(40, 300)] public string Script;
        public Vector2 Position;
        public Vector2 Size;
        public float UpdateInterval;

        public StatCounterRecord Copy()
        {
            return new StatCounterRecord
            {
                Name = Name,
                Type = Type,
                Filename = Filename,
                Script = Script,
                Position = Position,
                Size = Size,
                UpdateInterval = UpdateInterval
            };
        }
    }
}