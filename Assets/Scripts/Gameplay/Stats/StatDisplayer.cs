﻿using System.Collections;
using Blockstacker.Common.Alerts;
using Blockstacker.Gameplay;
using Blockstacker.Gameplay.Communication;
using NLua;
using NLua.Exceptions;
using TMPro;
using UnityEngine;

namespace Gameplay.Stats
{
    public class StatDisplayer : MonoBehaviour
    {
        [SerializeField] private StatCounter _counter;
        [SerializeField] private TMP_Text _displayText;
        [SerializeField] private GameTimer _timer;
        [SerializeField] private MediatorSO _mediator;
        [TextArea(10, 50)] [SerializeField] private string _statCounterScript;
        [Range(0, 5)] [SerializeField] private float _updateInterval = .1f;

        private Lua _luaState;
        private Coroutine _updateStatCor;

        private void Start()
        {
            _luaState = new Lua();
            _luaState["Stats"] = _counter.Stats;
            _updateStatCor = StartCoroutine(UpdateStatCor());
            _mediator.Register<GameRestartedMessage>(_ => HandleGameRestarted());
            _mediator.Register<GameLostMessage>(_ => HandleGameEnded());
        }

        private void HandleGameRestarted()
        {
            _luaState["Stats"] = _counter.Stats;
            if (_updateStatCor != null) return;
            _updateStatCor = StartCoroutine(UpdateStatCor());
        }

        private void HandleGameEnded()
        {
            StopCoroutine(_updateStatCor);
            _updateStatCor = null;
        }

        private IEnumerator UpdateStatCor()
        {
            while (true)
            {
                _luaState["CurrentTime"] = _timer.CurrentTimeAsSpan;
                string displayString;
                try
                {
                    displayString = _luaState.DoString(_statCounterScript)[0] as string;
                }
                catch (LuaException ex)
                {
                    _ = AlertDisplayer.Instance.ShowAlert(new Alert("Exception in stat script",
                        $"Exception in script:\n {ex.Message}", AlertType.Error));
                    displayString = ex.Message;
                }

                if (string.IsNullOrEmpty(displayString))
                    displayString = "Script not returning string";

                _displayText.text = displayString;
                yield return new WaitForSeconds(_updateInterval);
            }
        }
    }
}