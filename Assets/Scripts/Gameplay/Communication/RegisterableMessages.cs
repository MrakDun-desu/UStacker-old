
/************************************
RegisterableMessages.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Collections.Generic;

namespace UStacker.Gameplay.Communication
{
    public static class RegisterableMessages
    {
        public static readonly Dictionary<string, Type> Default = new()
        {
            {"CountdownTicked", typeof(CountdownTickedMessage)},
            {"GameEndConditionChanged", typeof(GameEndConditionChangedMessage)},
            {"GameStateChanged", typeof(GameStateChangedMessage)},
            {"HoldUsed", typeof(HoldUsedMessage)},
            {"InputAction", typeof(InputActionMessage)},
            {"LevelChanged", typeof(LevelChangedMessage)},
            {"LevelUpConditionChanged", typeof(LevelUpConditionChangedMessage)},
            {"PieceMoved", typeof(PieceMovedMessage)},
            {"PiecePlaced", typeof(PiecePlacedMessage)},
            {"PieceRotated", typeof(PieceRotatedMessage)},
            {"PieceSpawned", typeof(PieceSpawnedMessage)},
            {"ScoreChanged", typeof(ScoreChangedMessage)}
        };
    }
}
/************************************
end RegisterableMessages.cs
*************************************/
