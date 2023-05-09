
/************************************
GameStateChangedMessage.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UStacker.Gameplay.Enums;

namespace UStacker.Gameplay.Communication
{
    public readonly struct GameStateChangedMessage : IMidgameMessage
    {
        public readonly GameState PreviousState;
        public readonly GameState NewState;
        public readonly bool IsReplay;
        public double Time { get; }

        public GameStateChangedMessage(GameState previousState, GameState newState, double time, bool isReplay)
        {
            PreviousState = previousState;
            NewState = newState;
            Time = time;
            IsReplay = isReplay;
        }
    }
}
/************************************
end GameStateChangedMessage.cs
*************************************/
