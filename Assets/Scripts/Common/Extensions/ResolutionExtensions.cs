using System;
using UnityEngine;

namespace UStacker.Common.Extensions
{
    public static class ResolutionExtensions
    {
        public static bool IsEqualTo(this Resolution res1, Resolution res2)
        {
            return res1.width == res2.width &&
                   res1.height == res2.height &&
                   Math.Abs(res1.refreshRateRatio.value - res2.refreshRateRatio.value) < .5d;
        }
    }
}