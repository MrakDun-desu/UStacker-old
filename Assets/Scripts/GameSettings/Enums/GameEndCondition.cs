
/************************************
GameEndCondition.cs -- created by Marek Dančo (xdanco00)
*************************************/
namespace UStacker.GameSettings.Enums
{
    public enum GameEndCondition : byte
    {
        None = 0,
        LinesCleared = 1,
        GarbageLinesCleared = 2,
        PiecesPlaced = 3,
        Score = 4,
        Time = 5
    }
}
/************************************
end GameEndCondition.cs
*************************************/
