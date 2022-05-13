using System;
using Blockstacker.Gameplay;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using NLua;
using NLua.Exceptions;
using TMPro;
using UnityEngine;

namespace Gameplay.Stats
{
    public class EndGameStatDisplayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _displayText;
        [SerializeField] private GameManager _manager;
        [SerializeField] private GameSettingsSO _settings;
        [TextArea(10, 50)] [SerializeField] private string _statCounterScript;

        private Lua _luaState;
        
        private void OnEnable()
        {
            _luaState = new Lua();
            _luaState["stats"] = _manager.Replay.Stats;
            _luaState["mainStat"] = _settings.Objective.MainStat switch
            {
                MainStat.Score => "score",
                MainStat.Time => "time",
                MainStat.LinesCleared => "linesCleared",
                MainStat.CheeseLinesCleared => "cheeseLinesCleared",
                MainStat.PiecesUsed => "piecesUsed",
                _ => throw new ArgumentOutOfRangeException()
            };
            _luaState["gameEndTime"] = _manager.Replay.GameLength;
            
            string displayString;
            try
            {
                displayString = _luaState.DoString(_statCounterScript)[0] as string;
            }
            catch (LuaException ex)
            {
                displayString = $"Exception: {ex.Message}";
            }

            if (string.IsNullOrEmpty(displayString))
            {
                displayString = "Script not returning string";
            }

            _displayText.text = displayString;
        }
    }
}