namespace Blockstacker.Gameplay.Communication
{
    public record HoldUsedMessage : MidgameMessage
    {
        public bool WasSuccessful;
    }
}