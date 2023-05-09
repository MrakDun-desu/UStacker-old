using FishNet.Serializing;
using FishNet.Utility.Extension;

namespace FishNet.Managing.Timing
{
    public struct PreciseTick
    {
        /// <summary>
        ///     The current tick.
        /// </summary>
        public uint Tick;

        /// <summary>
        ///     Percentage into the next tick.
        /// </summary>
        public double Percent;

        public PreciseTick(uint tick, double percent)
        {
            Tick = tick;
            Percent = percent;
        }
    }

    public static class PreciseTickSerializer
    {
        public static void WritePreciseTick(this Writer writer, PreciseTick value)
        {
            writer.WriteUInt32(value.Tick, AutoPackType.Unpacked);
            /* No reason percent should exist beyond these values, but better to be safe.
             * There is also no double clamp in Unity so... */
            var percent = MathFN.ClampDouble(value.Percent, 0d, 1f);
            var percentByte = (byte) (percent * 100);
            writer.WriteByte(percentByte);
        }

        public static PreciseTick ReadPreciseTick(this Reader reader)
        {
            var tick = reader.ReadUInt32(AutoPackType.Unpacked);
            var percentByte = reader.ReadByte();
            var percent = MathFN.ClampDouble(percentByte / 100f, 0d, 1d);
            return new PreciseTick(tick, percent);
        }
    }
}