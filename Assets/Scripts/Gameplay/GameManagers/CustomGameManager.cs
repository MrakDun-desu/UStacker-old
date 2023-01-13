using System;
using UStacker.Common;
using UStacker.Common.Alerts;
using UStacker.Gameplay.Communication;
using NLua;
using NLua.Exceptions;
using UnityEngine;
using UStacker.Gameplay.Timing;

namespace UStacker.Gameplay.GameManagers
{
    public class CustomGameManager : MonoBehaviour, IGameManager
    {
        private const string STARTING_LEVEL_NAME = "StartingLevel";
        private const string BOARD_INTERFACE_NAME = "Board";
        private Lua _luaState;
        private MediatorSO _mediator;
        private uint _startingLevel;
        private GameStateManager _stateManager;
        private double? _currentMessageTime;
        private GameTimer _timer;

        public void Initialize(string startingLevel, MediatorSO mediator)
        {
            uint.TryParse(startingLevel, out _startingLevel);
            _mediator = mediator;
        }

        public void CustomInitialize(GameStateManager stateManager, Board board, GameTimer timer, string script)
        {
            _stateManager = stateManager;
            _timer = timer;

            _luaState = CreateLua.WithAllPrerequisites();
            _luaState[STARTING_LEVEL_NAME] = _startingLevel;
            _luaState[BOARD_INTERFACE_NAME] = new GameManagerBoardInterface(board);

            RegisterMethod(nameof(LoseGame));
            RegisterMethod(nameof(EndGame));
            RegisterMethod(nameof(SetScore));
            RegisterMethod(nameof(SetLevel));
            RegisterMethod(nameof(SetGravity));
            RegisterMethod(nameof(SetLockDelay));
            RegisterMethod(nameof(SetGameEndCondition));
            RegisterMethod(nameof(SetLevelUpCondition));

            LuaTable events = null;
            try
            {
                var returnedValue = _luaState.DoString(script);
                if (returnedValue.Length == 0) return;
                if (returnedValue[0] is LuaTable eventTable) events = eventTable;
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
                    if (message is MidgameMessage m)
                        _currentMessageTime = m.Time;
                    else
                        _currentMessageTime = null;

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

        private void RegisterMethod(string methodName)
        {
            _luaState.RegisterFunction(methodName, this, GetType().GetMethod(methodName));
        }

        #region Methods supplied to LuaState

        public void EndGame()
        {
            _stateManager.EndGame(_currentMessageTime ?? _timer.CurrentTime);
        }

        public void LoseGame()
        {
            _stateManager.LoseGame(_currentMessageTime ?? _timer.CurrentTime);
        }

        public void SetScore(object score)
        {
            _mediator.Send(new ScoreChangedMessage(Convert.ToInt64(score), _currentMessageTime ?? _timer.CurrentTime));
        }

        public void SetLevel(object level)
        {
            _mediator.Send(new LevelChangedMessage(level.ToString(), _currentMessageTime ?? _timer.CurrentTime));
        }

        public void SetLevelUpCondition(object current, object total, string condName)
        {
            _mediator.Send(new LevelUpConditionChangedMessage(
                _currentMessageTime ?? _timer.CurrentTime,
                Convert.ToDouble(total),
                Convert.ToDouble(current),
                condName));
        }

        public void SetGameEndCondition(object current, object total, string condName)
        {
            _mediator.Send(new GameEndConditionChangedMessage(
                _currentMessageTime ?? _timer.CurrentTime,
                Convert.ToDouble(total),
                Convert.ToDouble(current),
                condName));
        }

        public void SetGravity(object newGravity)
        {
            _mediator.Send(new GravityChangedMessage(Convert.ToDouble(newGravity),
                _currentMessageTime ?? _timer.CurrentTime));
        }

        public void SetLockDelay(object newDelay)
        {
            _mediator.Send(new LockDelayChangedMessage(Convert.ToDouble(newDelay),
                _currentMessageTime ?? _timer.CurrentTime));
        }

        #endregion
    }
}