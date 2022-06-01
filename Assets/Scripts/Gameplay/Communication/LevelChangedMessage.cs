namespace Blockstacker.Gameplay.Communication
{
    public record LevelChangedMessage : Message
    {
        public uint Level;
    }
}