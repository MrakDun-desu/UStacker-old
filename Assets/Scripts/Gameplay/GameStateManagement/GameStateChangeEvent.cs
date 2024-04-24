
/************************************
GameStateChangeEvent.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UStacker.Gameplay.Enums;

namespace UStacker.Gameplay.GameStateManagement
{
    [Serializable]
    public class GameStateChangeEvent
    {
        [UsedImplicitly] // used only for ease of use in Unity arrays
        [SerializeField]
        private string _name;

        public GameState PreviousState = GameState.Any;
        public GameState NewState = GameState.Any;
        public bool WorkInReplay = true;
        public bool WorkInGame = true;
        public UnityEvent OnStateChanged;
    }
}
/************************************
end GameStateChangeEvent.cs
*************************************/
