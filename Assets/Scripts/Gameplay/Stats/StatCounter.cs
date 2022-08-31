using System;
using System.Collections.Generic;
using Blockstacker.Common.Alerts;
using Blockstacker.Common.Extensions;
using Blockstacker.Gameplay.Communication;
using Blockstacker.GameSettings;
using Blockstacker.GlobalSettings.StatCounting;
using NLua;
using NLua.Exceptions;
using TMPro;
using UnityEngine;

namespace Blockstacker.Gameplay.Stats
{
    public class StatCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private CustomStatEntry _statEntry = null;
        private Lua _luaState;

        public GameSettingsSO Settings;
        public MediatorSO Mediator;
        public StatCounterRecord StatCounterRecord { get; set; }

        private readonly Dictionary<string, Type> RegisterableEvents = new()
        {
            {"GameEnded", typeof(GameEndedMessage)},
            {"GameStarted", typeof(GameStartedMessage)},
            {"HoldUsed", typeof(HoldUsedMessage)},
            {"InputAction", typeof(InputActionMessage)},
            {"LevelChanged", typeof(LevelChangedMessage)},
            {"PieceMoved", typeof(PieceMovedMessage)},
            {"PiecePlaced", typeof(PiecePlacedMessage)},
            {"PieceRotated", typeof(PieceRotatedMessage)},
            {"PieceSpawned", typeof(PieceSpawnedMessage)},
        };

        private const string STARTING_LEVEL_VAR_NAME = "StartingLevel";
        private const string IS_LEVELLING_VAR_NAME = "IsLevellingSystem";
        private const string UTILITY_VAR_NAME = "StatUtility";

        private void Start()
        {
            if (string.IsNullOrEmpty(StatCounterRecord.Script))
                return;
            _luaState = new Lua();
            _luaState.RestrictMaliciousFunctions();

            _luaState[STARTING_LEVEL_VAR_NAME] = Settings.Rules.Gravity.StartingLevel;
            _luaState[IS_LEVELLING_VAR_NAME] = StatCounterRecord.IsLevellingSystem;
            // _luaState[UTILITY_VAR_NAME] TODO
            
            LuaTable events = null;

            try
            {
                var returnedValue = _luaState.DoString(StatCounterRecord.Script);
                if (returnedValue.Length == 0) return;
                if (returnedValue[0] is LuaTable eventTable)
                {
                    events = eventTable;
                }
            }
            catch (LuaException ex)
            {
                _ = AlertDisplayer.Instance.ShowAlert(new Alert(
                    $"Error reading stat script {StatCounterRecord.Name}",
                    $"Lua error: {ex.Message}",
                    AlertType.Error));
                return;
            }

            if (events is null) return;

            foreach (var entry in RegisterableEvents)
            {
                if (events[entry.Key] is not LuaFunction function) continue;

                void Action(Message message)
                {
                    object[] output;
                    try
                    {
                        output = function.Call(message);
                    }
                    catch (LuaException ex)
                    {
                        _ = AlertDisplayer.Instance.ShowAlert(new Alert(
                            $"Error executing stat script {StatCounterRecord.Name}",
                            $"Lua error: {ex.Message}",
                            AlertType.Error));
                        return;
                    }

                    if (output is null || output.Length == 0 || output[0] is not string outputString) return;

                    _text.text = outputString;
                }
                
                Mediator.Register((Action<Message>) Action, entry.Value);
            }
        }
    }
}