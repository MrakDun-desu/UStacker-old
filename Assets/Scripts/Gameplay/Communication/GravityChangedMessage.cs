namespace Blockstacker.Gameplay.Communication
{
    public record GravityChangedMessage : Message
    {
        public double Gravity;
    }
}