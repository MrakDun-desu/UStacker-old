using System;
using UnityEngine;

namespace FishNet.Object
{
    [Serializable]
    public struct TransformProperties
    {
        public readonly Vector3 LocalScale;
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;

        public TransformProperties(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            Position = position;
            Rotation = rotation;
            LocalScale = localScale;
        }
    }
}