namespace Blockstacker.Gameplay.Communication
{
    public record HoldUsedMessage : MidgameMessage
    {
        public readonly bool WasSuccessful;

        public HoldUsedMessage(bool wasSuccessful, double time) : base(time)
        {
            WasSuccessful = wasSuccessful;
        }
    }
}