
/************************************
HoldUsedMessage.cs -- created by Marek Dančo (xdanco00)
*************************************/
namespace UStacker.Gameplay.Communication
{
    public readonly struct HoldUsedMessage : IMidgameMessage
    {
        public readonly bool WasSuccessful;
        public double Time { get; }

        public HoldUsedMessage(bool wasSuccessful, double time)
        {
            Time = time;
            WasSuccessful = wasSuccessful;
        }
    }
}
/************************************
end HoldUsedMessage.cs
*************************************/
