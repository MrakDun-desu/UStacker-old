﻿using UnityEngine;

namespace Blockstacker.Common.Extensions
{
    public static class RectTransformExtensions
    {
        public static Rect GetWorldSpaceRect(this RectTransform transform)
        {
            var corners = new Vector3[4];
            transform.GetWorldCorners(corners);
            var pos = corners[0];

            var lossyScale = transform.lossyScale;
            var rect = transform.rect;
            var size = new Vector2(
                lossyScale.x * rect.size.x,
                lossyScale.y * rect.size.y);

            return new Rect(pos, size);
        }
    }
}