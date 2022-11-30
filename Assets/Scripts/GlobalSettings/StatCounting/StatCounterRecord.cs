using System;
using UnityEngine;

namespace Blockstacker.GlobalSettings.StatCounting
{
    [Serializable]
    public class StatCounterRecord : IEquatable<StatCounterRecord>
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

        public bool Equals(StatCounterRecord other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Type == other.Type && Filename == other.Filename && Script == other.Script &&
                   Position.Equals(other.Position) && Size.Equals(other.Size) &&
                   UpdateInterval.Equals(other.UpdateInterval);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((StatCounterRecord) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, (int) Type, Filename, Script, Position, Size, UpdateInterval);
        }
    }
}