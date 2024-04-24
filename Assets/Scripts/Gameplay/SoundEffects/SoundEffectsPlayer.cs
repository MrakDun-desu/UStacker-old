
/************************************
SoundEffectsPlayer.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Collections.Generic;
using NLua;
using NLua.Exceptions;
using UnityEngine;
using UStacker.Common.Alerts;
using UStacker.Common.LuaApi;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;
using UStacker.GlobalSettings.Music;

namespace UStacker.Gameplay.SoundEffects
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffectsPlayer : MonoBehaviour, IDisposable
    {
        [SerializeField] private AudioClipCollection _defaultEffects = new();
        [SerializeField] private Mediator _mediator;
        [SerializeField] private AudioSource _audioSource;

        public bool RepressSfx;
        private readonly List<string> _playedInThisUpdate = new();
        private Lua _luaState;

        private void Awake()
        {
            if (!TryRegisterCustomFunctions())
                RegisterDefaultFunctions();
            SoundPackLoader.SoundPackChanged += Reload;
        }

        private void LateUpdate()
        {
            _playedInThisUpdate.Clear();
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            _mediator.Unregister<PiecePlacedMessage>(HandlePiecePlaced);
            _mediator.Unregister<PieceRotatedMessage>(HandlePieceRotated);
            _mediator.Unregister<PieceMovedMessage>(HandlePieceMoved);
            _mediator.Unregister<HoldUsedMessage>(HandleHoldUsed);
            _mediator.Unregister<PieceSpawnedMessage>(HandlePieceSpawned);
            _mediator.Unregister<CountdownTickedMessage>(HandleCountdownTicked);
            _mediator.Unregister<GameStateChangedMessage>(HandleGameStateChanged);
            _luaState?.Dispose();
        }

        private void Reload()
        {
            Dispose();
            if (!TryRegisterCustomFunctions())
                RegisterDefaultFunctions();
        }

        private void RegisterDefaultFunctions()
        {
            _mediator.Register<PiecePlacedMessage>(HandlePiecePlaced);
            _mediator.Register<PieceRotatedMessage>(HandlePieceRotated);
            _mediator.Register<PieceMovedMessage>(HandlePieceMoved);
            _mediator.Register<HoldUsedMessage>(HandleHoldUsed);
            _mediator.Register<PieceSpawnedMessage>(HandlePieceSpawned);
            _mediator.Register<CountdownTickedMessage>(HandleCountdownTicked);
            _mediator.Register<GameStateChangedMessage>(HandleGameStateChanged);
        }

        private bool TryRegisterCustomFunctions()
        {
            if (string.IsNullOrEmpty(SoundPackLoader.SoundEffectsScript))
                return false;

            var myType = GetType();
            _luaState = CreateLua.WithAllPrerequisites(out _);
            _luaState.RegisterFunction(nameof(Play), this, myType.GetMethod(nameof(Play)));
            _luaState.RegisterFunction(nameof(PlayAsAnnouncer), this, myType.GetMethod(nameof(PlayAsAnnouncer)));
            LuaTable events = null;
            try
            {
                var returnedValue = _luaState.DoString(SoundPackLoader.SoundEffectsScript);
                if (returnedValue.Length == 0) return false;
                if (returnedValue[0] is LuaTable eventTable) events = eventTable;
            }
            catch (LuaException ex)
            {
                AlertDisplayer.ShowAlert(new Alert(
                    "Error reading sound effects script!",
                    $"Switching to default sound effects.\nLua error: {ex.Message}",
                    AlertType.Error
                ));
                return false;
            }

            if (events is null) return false;

            foreach (var eventNameObj in events.Keys)
            {
                if (eventNameObj is not string eventName)
                {
                    AlertDisplayer.ShowAlert(new Alert(
                        "Invalid event name!",
                        $"Custom sound effects script tried registering an invalid event {eventNameObj}",
                        AlertType.Warning));
                    continue;
                }

                if (events[eventNameObj] is not LuaFunction function)
                {
                    AlertDisplayer.ShowAlert(new Alert(
                        "Invalid event handler!",
                        $"Custom sound effects script tried registering an invalid handler for event {eventName}",
                        AlertType.Warning));
                    continue;
                }

                if (!RegisterableMessages.Default.ContainsKey(eventName))
                {
                    AlertDisplayer.ShowAlert(new Alert(
                        "Invalid event name!",
                        $"Custom sound effects script tried registering an invalid event {eventName}",
                        AlertType.Warning));
                    continue;
                }

                void Action(IMessage message)
                {
                    object[] output = null;
                    try
                    {
                        output = function.Call(message);
                    }
                    catch (LuaException ex)
                    {
                        AlertDisplayer.ShowAlert(new Alert(
                            "Error executing user code!",
                            $"Error executing sound effects script.\nLua error: {ex.Message}",
                            AlertType.Error
                        ));
                    }

                    if (output is null) return;

                    foreach (var obj in output)
                    {
                        if (obj is not string clipName) return;

                        TryPlayClip(clipName);
                    }
                }

                _mediator.Register((Action<IMessage>) Action, RegisterableMessages.Default[eventName]);
            }

            return true;
        }

        private void HandleGameStateChanged(GameStateChangedMessage message)
        {
            switch (message.NewState)
            {
                case GameState.Lost:
                    TryPlayClip("death");
                    break;
                case GameState.Ended:
                    TryPlayClip("finish");
                    break;
                case GameState.Unset or
                    GameState.Any or
                    GameState.Initializing or
                    GameState.StartCountdown or
                    GameState.Running or
                    GameState.Paused or
                    GameState.ResumeCountdown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleHoldUsed(HoldUsedMessage message)
        {
            if (message.WasSuccessful)
                TryPlayClip("hold");
        }

        private void HandleCountdownTicked(CountdownTickedMessage message)
        {
            string countdownKey = null;
            var found = false;
            for (var i = (int) message.RemainingTicks; i >= 0; i--)
            {
                countdownKey = $"countdown{i}";
                if (SoundPackLoader.SoundEffects.ContainsKey(countdownKey))
                {
                    found = true;
                    break;
                }

                if (_defaultEffects.ContainsKey(countdownKey))
                {
                    found = true;
                    break;
                }
            }

            if (found)
                TryPlayClip(countdownKey);
        }

        private void HandlePieceSpawned(PieceSpawnedMessage message)
        {
            if (!string.IsNullOrEmpty(message.NextPiece))
                TryPlayClip(message.NextPiece[^1].ToString().ToLowerInvariant());
        }

        private void HandlePiecePlaced(PiecePlacedMessage message)
        {
            if (message.WasAllClear)
                TryPlayClip("allclear");

            switch (message.LinesCleared)
            {
                case 0:
                    if (message.BrokenCombo) TryPlayClip("combobreak");

                    TryPlayClip("floor");
                    break;
                case < 4:
                    if (message.WasSpin || message.WasSpinMini)
                    {
                        switch (message.CurrentCombo)
                        {
                            case 0:
                                TryPlayClip("clearspin");
                                break;
                            case < 16:
                                TryPlayClip($"combo_{message.CurrentCombo}_power");
                                break;
                            case >= 16:
                                TryPlayClip("combo_16_power");
                                break;
                        }
                    }
                    else
                    {
                        if (message.BrokenBackToBack)
                            TryPlayClip("btb_break");

                        switch (message.CurrentCombo)
                        {
                            case 0:
                                TryPlayClip("clearline");
                                break;
                            case < 16:
                                TryPlayClip($"combo_{message.CurrentCombo}");
                                break;
                            case >= 16:
                                TryPlayClip("combo_16");
                                break;
                        }
                    }

                    break;
                case 4:
                    switch (message.CurrentCombo)
                    {
                        case 0:
                            TryPlayClip("clearquad");
                            break;
                        case < 16:
                            TryPlayClip($"combo_{message.CurrentCombo}_power");
                            break;
                        case >= 16:
                            TryPlayClip("combo_16_power");
                            break;
                    }

                    break;
            }
        }

        private void HandlePieceRotated(PieceRotatedMessage message)
        {
            if (message.WasSpin || message.WasSpinMini)
                TryPlayClip("spin");
            else
                TryPlayClip("rotate");
        }

        private void HandlePieceMoved(PieceMovedMessage message)
        {
            if (message.X != 0)
                TryPlayClip("move");
            else if (message.Y != 0 && message.WasSoftDrop)
                TryPlayClip("softdrop");
            else if (message.WasHardDrop)
                TryPlayClip("harddrop");
        }

        public void PlayAsAnnouncer(string clipName)
        {
            if (_playedInThisUpdate.Contains(clipName) || RepressSfx)
                return;

            if (!SoundPackLoader.SoundEffects.TryGetValue(clipName, out var clip))
                if (!_defaultEffects.TryGetValue(clipName, out clip))
                {
                    AlertDisplayer.ShowAlert(new Alert(
                        "Clip not found!",
                        $"Sound effect with a name {clipName} was not found.",
                        AlertType.Warning
                    ));
                    return;
                }

            _audioSource.clip = clip;
            _audioSource.Play();

            _playedInThisUpdate.Add(clipName);
        }

        public void Play(string clipName)
        {
            TryPlayClip(clipName);
        }

        private void TryPlayClip(string clipName)
        {
            if (RepressSfx || _playedInThisUpdate.Contains(clipName))
                return;

            if (!SoundPackLoader.SoundEffects.TryGetValue(clipName, out var clip))
                if (!_defaultEffects.TryGetValue(clipName, out clip))
                {
                    AlertDisplayer.ShowAlert(new Alert(
                        "Clip not found!",
                        $"Sound effect with a name {clipName} was not found.",
                        AlertType.Warning
                    ));
                    return;
                }

            _audioSource.PlayOneShot(clip);
            _playedInThisUpdate.Add(clipName);
        }
    }
}
/************************************
end SoundEffectsPlayer.cs
*************************************/
