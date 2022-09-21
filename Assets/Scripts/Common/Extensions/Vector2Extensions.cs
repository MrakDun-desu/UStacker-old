using UnityEngine;

namespace Blockstacker.Common.Extensions
{
    public static class Vector2Extensions
    {
        public static bool IsInside(this Vector2 target, Vector2 first, Vector2 second)
        {
            Vector2 biggerVec;
            Vector2 smallerVec;
            if (first.x > second.x)
            {
                biggerVec.x = first.x;
                smallerVec.x = second.x;
            }
            else
            {
                biggerVec.x = second.x;
                smallerVec.x = first.x;
            }

            if (first.y > second.y)
            {
                biggerVec.y = first.y;
                smallerVec.y = second.y;
            }
            else
            {
                biggerVec.y = second.y;
                smallerVec.y = first.y;
            }

            return target.x < biggerVec.x && 
                   target.y < biggerVec.y &&
                   target.x > smallerVec.x && 
                   target.y > smallerVec.y;
        }
    }
}