using System;
using System.Collections.Generic;

namespace Blockstacker.Gameplay.Communication
{
    public static class RegisterableMessages
    {
        public static readonly Dictionary<string, Type> Default = new()
        {
            {
                "CountdownTicked", typeof(CountdownTickedMessage)
            },
            {
                "GameEnded", typeof(GameEndedMessage)
            },
            {
                "GameLost", typeof(GameLostMessage)
            },
            {
                "GamePaused", typeof(GamePausedMessage)
            },
            {
                "GameRestarted", typeof(GameRestartedMessage)
            },
            {
                "GameResumed", typeof(GameResumedMessage)
            },
            {
                "GameStarted", typeof(GameStartedMessage)
            },
            {
                "HoldUsed", typeof(HoldUsedMessage)
            },
            {
                "InputAction", typeof(InputActionMessage)
            },
            {
                "LevelChanged", typeof(LevelChangedMessage)
            },
            {
                "PieceMoved", typeof(PieceMovedMessage)
            },
            {
                "PiecePlaced", typeof(PiecePlacedMessage)
            },
            {
                "PieceRotated", typeof(PieceRotatedMessage)
            },
            {
                "PieceSpawned", typeof(PieceSpawnedMessage)
            },
            {
                "ScoreChanged", typeof(ScoreChangedMessage)
            },
            {
                "GameEndConditionChanged", typeof(GameEndConditionChangedMessage)
            },
            {
                "LevelUpConditionChanged", typeof(LevelUpConditionChangedMessage)
            }
        };
    }
}