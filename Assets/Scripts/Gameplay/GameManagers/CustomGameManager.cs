﻿using System;
using Blockstacker.Common.Alerts;
using Blockstacker.Common.Extensions;
using Blockstacker.Gameplay.Communication;
using UnityEngine;
using NLua;
using NLua.Exceptions;

namespace Blockstacker.Gameplay.GameManagers
{
    public class CustomGameManager : MonoBehaviour, IGameManager
    {
        private Lua _luaState;
        private GameStateManager _stateManager;
        private MediatorSO _mediator;
        private uint _startingLevel;

        private const string STARTING_LEVEL_NAME = "StartingLevel";
        private const string BOARD_INTERFACE_NAME = "Board";

        public void CustomInitialize(GameStateManager stateManager, Board board, string script)
        {
            _stateManager = stateManager;

            _luaState = new Lua();
            _luaState.RestrictMaliciousFunctions();
            _luaState[STARTING_LEVEL_NAME] = _startingLevel;
            _luaState[BOARD_INTERFACE_NAME] = new GameManagerBoardInterface(board);
            
            RegisterMethod(nameof(LoseGame));
            RegisterMethod(nameof(EndGame));
            RegisterMethod(nameof(SetScore));
            RegisterMethod(nameof(SetLevel));
            RegisterMethod(nameof(SetGameEndCondition));
            RegisterMethod(nameof(SetLevelUpCondition));
            
            LuaTable events = null;
            try
            {
                var returnedValue = _luaState.DoString(script);
                if (returnedValue.Length == 0) return;
                if (returnedValue[0] is LuaTable eventTable)
                {
                    events = eventTable;
                }
            }
            catch (LuaException ex)
            {
                _ = AlertDisplayer.Instance.ShowAlert(new Alert(
                    "Error reading custom game manager script!",
                    $"Game manager won't be functional. Lua exception: {ex.Message}",
                    AlertType.Error
                ));
                enabled = false;
                return;
            }

            if (events is null) return;

            foreach (var entry in RegisterableMessages.Default)
            {
                if (events[entry.Key] is not LuaFunction function) continue;

                void Action(Message message)
                {
                    if (!enabled) return;
                    try
                    {
                        function.Call(message);
                    }
                    catch (LuaException ex)
                    {
                        _ = AlertDisplayer.Instance.ShowAlert(new Alert(
                            "Error executing custom game manager script!",
                            $"Game manager will be turned off. Lua exception: {ex.Message}",
                            AlertType.Error
                        ));
                        enabled = false;
                    }
                }

                _mediator.Register((Action<Message>) Action, entry.Value);
            }
        }
        
        public void Initialize(string startingLevel, MediatorSO mediator)
        {
            uint.TryParse(startingLevel, out _startingLevel);
            _mediator = mediator;
        }

        private void RegisterMethod(string methodName)
        {
            _luaState.RegisterFunction(methodName, this, GetType().GetMethod(methodName));
        }

        #region Methods supplied to LuaState
        
        public void EndGame(double time) => _stateManager.EndGame(time);

        public void LoseGame(double time) => _stateManager.LoseGame(time);

        public void SetScore(object score, double time) =>
            _mediator.Send(new ScoreChangedMessage(Convert.ToInt64(score), time));

        public void SetLevel(object level, double time) =>
            _mediator.Send(new LevelChangedMessage(level.ToString(), time));

        public void SetLevelUpCondition(object current, object total, double time, string condName) =>
            _mediator.Send(new LevelUpConditionChangedMessage(
                time,
                Convert.ToDouble(total),
                Convert.ToDouble(current),
                condName));

        public void SetGameEndCondition(object current, object total, double time, string condName) =>
            _mediator.Send(new GameEndConditionChangedMessage(
                time,
                Convert.ToDouble(total),
                Convert.ToDouble(current),
                condName));

        #endregion
    }
}