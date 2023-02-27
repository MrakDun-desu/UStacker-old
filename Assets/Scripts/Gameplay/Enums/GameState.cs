namespace UStacker.Gameplay.Enums
{
    public enum GameState : byte
    {
        Unset = 0, // before the scene loads
        Any = 1, // used only for registering events
        Initializing = 2, // from game start/restart until countdown start
        GameStartCountdown = 7, // counting down start of the game
        Running = 3, // game is running and player's inputs are accepted
        Paused = 4, // game is in progress, but not running
        GameResumeCountdown = 8, // counting down resuming of the game
        Ended = 5, // game has successfully ended with game end condition
        Lost = 6, // game has ended with player failure
    }
}