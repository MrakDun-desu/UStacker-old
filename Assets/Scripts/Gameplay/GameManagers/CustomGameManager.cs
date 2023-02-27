using System;
using UStacker.Gameplay.Communication;
using NLua;
using NLua.Exceptions;
using UnityEngine;
using UnityEngine.Events;
using UStacker.Common.Alerts;
using UStacker.Common.LuaApi;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.Timing;
using Random = UStacker.Common.Random;

namespace UStacker.Gameplay.GameManagers
{
    public class CustomGameManager : MonoBehaviour, IGameManager
    {
        private const string STARTING_LEVEL_NAME = "StartingLevel";
        private const string BOARD_INTERFACE_NAME = "Board";
        private Lua _luaState;
        private Mediator _mediator;
        private uint _startingLevel;
        private UnityEvent<double> EndEvent;
        private UnityEvent<double> LoseEvent;
        private double? _currentMessageTime;
        private GameTimer _timer;
        private Random _random;
        private long _currentScore;

        public void Initialize(string startingLevel, Mediator mediator)
        {
            uint.TryParse(startingLevel, out _startingLevel);
            _mediator = mediator;
            _mediator.Register<GameStateChangedMessage>(HandleGameStateChange);
            _mediator.Register<SeedSetMessage>(OnSeedSet);
        }

        private void ResetState()
        {
            _currentScore = 0;
            _mediator.Send(new ScoreChangedMessage(_currentScore, 0));
        }

        private void HandleGameStateChange(GameStateChangedMessage message)
        {
            if (message.NewState is not GameState.Initializing)
                return;
            
            ResetState();
        }

        private void OnSeedSet(SeedSetMessage message)
        {
            _random.State = message.Seed;
        }

        public void CustomInitialize(
            Board board, 
            GameTimer timer, 
            string script, 
            UnityEvent<double> endEvent,
            UnityEvent<double> loseEvent, 
            ulong seed)
        {
            _timer = timer;
            EndEvent = endEvent;
            LoseEvent = loseEvent;

            _luaState = CreateLua.WithAllPrerequisites(out _random);
            _random.State = seed;
            _luaState[STARTING_LEVEL_NAME] = _startingLevel;
            _luaState[BOARD_INTERFACE_NAME] = new GameManagerBoardInterface(board);

            RegisterMethod(nameof(LoseGame));
            RegisterMethod(nameof(EndGame));
            RegisterMethod(nameof(AddScore));
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
                _mediator.Send(new GameCrashedMessage($"Custom game manager crashed! Lua exception: {ex.Message}"));
                return;
            }

            if (events is null) return;

            foreach (var eventNameObj in events.Keys)
            {
                if (eventNameObj is not string eventName)
                {
                    AlertDisplayer.Instance.ShowAlert(new Alert(
                        "Invalid event name!",
                        $"Custom game manager tried registering an invalid event {eventNameObj}",
                        AlertType.Warning));
                    continue;
                }

                if (events[eventNameObj] is not LuaFunction function)
                {
                    AlertDisplayer.Instance.ShowAlert(new Alert(
                        "Invalid event handler!",
                        $"Custom game manager tried registering an invalid handler for event {eventName}",
                        AlertType.Warning));
                    continue;
                }

                if (!RegisterableMessages.Default.ContainsKey(eventName))
                {
                    AlertDisplayer.Instance.ShowAlert(new Alert(
                        "Invalid event name!",
                        $"Custom game manager tried registering an invalid event {eventName}",
                        AlertType.Warning));
                    continue;
                }

                void Action(IMessage message)
                {
                    if (message is IMidgameMessage m)
                        _currentMessageTime = m.Time;
                    else
                        _currentMessageTime = null;

                    try
                    {
                        function.Call(message);
                    }
                    catch (LuaException ex)
                    {
                        _mediator.Send(
                            new GameCrashedMessage($"Custom game manager crashed! Lua exception: {ex.Message}"));
                    }
                }

                _mediator.Register((Action<IMessage>) Action, RegisterableMessages.Default[eventName]);
            }
        }

        private void RegisterMethod(string methodName)
        {
            _luaState.RegisterFunction(methodName, this, GetType().GetMethod(methodName));
        }

        #region Methods supplied to LuaState

        public void EndGame()
        {
            EndEvent.Invoke(_currentMessageTime ?? _timer.CurrentTime);
        }

        public void LoseGame()
        {
            LoseEvent.Invoke(_currentMessageTime ?? _timer.CurrentTime);
        }

        public void AddScore(object score)
        {
            _currentScore += Convert.ToInt64(score);
            _mediator.Send(new ScoreChangedMessage(_currentScore, _currentMessageTime ?? _timer.CurrentTime));
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