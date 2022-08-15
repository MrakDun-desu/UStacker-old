namespace Blockstacker.GameSettings.Enums
{
    public enum AllowedSpins : byte
    {
        None = 0,
        ISpins = 1,
        TSpins = 1 << 1,
        LSpins = 1 << 2,
        JSpins = 1 << 3,
        OSpins = 1 << 4,
        SSpins = 1 << 5,
        ZSpins = 1 << 6,
        All =    0b0111111,
        Stupid = 0b1111111
    }
}