using UnityEngine;

namespace Blockstacker.Common.Extensions
{
    public static class ResolutionExtensions
    {
        public static bool IsEqualTo(this Resolution res1, Resolution res2)
        {
            return res1.width == res2.width &&
                   res1.height == res2.height &&
                   res1.refreshRate == res2.refreshRate;
        }
    }
}