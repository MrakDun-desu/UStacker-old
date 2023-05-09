namespace FishNet.Utility.Extension
{
    public static class MathFN
    {
        /// <summary>
        ///     Returns a clamped SBytte.
        /// </summary>
        public static sbyte ClampSByte(long value, sbyte min, sbyte max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return (sbyte) value;
        }

        /// <summary>
        ///     Returns a clamped double.
        /// </summary>
        public static double ClampDouble(double value, double min, double max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
    }
}