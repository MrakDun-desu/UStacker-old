using JetBrains.Annotations;

namespace UStacker.Gameplay.Communication
{
    public readonly struct GameEndedMessage : IMessage
    {
        [UsedImplicitly]
        public readonly double EndTime;

        public GameEndedMessage(double endTime)
        {
            EndTime = endTime;
        }
    }
}