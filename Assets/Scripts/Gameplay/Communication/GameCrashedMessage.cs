namespace UStacker.Gameplay.Communication
{
    public readonly struct GameCrashedMessage : IMessage
    {
        public readonly string CrashMessage;

        public GameCrashedMessage(string crashMessage)
        {
            CrashMessage = crashMessage;
        }
    }
}