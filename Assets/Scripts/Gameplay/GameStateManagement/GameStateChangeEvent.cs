using System;
using UnityEngine;
using UnityEngine.Events;
using UStacker.Gameplay.Enums;

namespace UStacker.Gameplay.GameStateManagement
{
    [Serializable]
    public class GameStateChangeEvent
    {
        [SerializeField] private string _name;
        public GameState PreviousState = GameState.Any;
        public GameState NewState = GameState.Any;
        public bool WorkInReplay = true;
        public bool WorkInGame = true;
        public UnityEvent OnStateChanged;
    }
}