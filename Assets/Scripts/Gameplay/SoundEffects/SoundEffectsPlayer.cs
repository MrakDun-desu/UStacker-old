using System;
using System.Collections.Generic;
using NLua;
using NLua.Exceptions;
using UnityEngine;
using UStacker.Common.Alerts;
using UStacker.Common.LuaApi;
using UStacker.Gameplay.Communication;
using UStacker.GlobalSettings.Music;

namespace UStacker.Gameplay.SoundEffects
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffectsPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClipCollection _defaultEffects = new();
        [SerializeField] private Mediator _mediator;
        
        public bool RepressSfx;
        private AudioSource _audioSource;
        private Lua _luaState;
        private readonly List<string> _playedInThisUpdate = new();

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (!TryRegisterCustomFunctions())
                RegisterDefaultFunctions();
        }

        private void LateUpdate()
        {
            _playedInThisUpdate.Clear();
        }

        private void RegisterDefaultFunctions()
        {
            _mediator.Register<PiecePlacedMessage>(HandlePiecePlaced);
            _mediator.Register<PieceRotatedMessage>(HandlePieceRotated);
            _mediator.Register<PieceMovedMessage>(HandlePieceMoved);
            _mediator.Register<HoldUsedMessage>(HandleHoldUsed);
            _mediator.Register<PieceSpawnedMessage>(HandlePieceSpawned);
            _mediator.Register<CountdownTickedMessage>(HandleCountdownTicked);
            _mediator.Register<GameLostMessage>(_ => TryPlayClip("death"));
            _mediator.Register<GameEndedMessage>(_ => TryPlayClip("finish"));
        }

        private bool TryRegisterCustomFunctions()
        {
            if (string.IsNullOrEmpty(SoundPackLoader.SoundEffectsScript))
                return false;

            _luaState = CreateLua.WithAllPrerequisites(out _);
            _luaState.RegisterFunction(nameof(Play), this, GetType().GetMethod(nameof(Play)));
            _luaState.RegisterFunction(nameof(PlayAsAnnouncer), this, GetType().GetMethod(nameof(PlayAsAnnouncer)));
            LuaTable events = null;
            try
            {
                var returnedValue = _luaState.DoString(SoundPackLoader.SoundEffectsScript);
                if (returnedValue.Length == 0) return false;
                if (returnedValue[0] is LuaTable eventTable) events = eventTable;
            }
            catch (LuaException ex)
            {
                AlertDisplayer.Instance.ShowAlert(new Alert(
                    "Error reading sound effects script!",
                    $"Switching to default sound effects.\nLua error: {ex.Message}",
                    AlertType.Error
                ));
                return false;
            }

            if (events is null) return false;

            foreach (var entry in RegisterableMessages.Default)
            {
                if (events[entry.Key] is not LuaFunction function) continue;

                void Action(IMessage message)
                {
                    object[] output = null;
                    try
                    {
                        output = function.Call(message);
                    }
                    catch (LuaException ex)
                    {
                        AlertDisplayer.Instance.ShowAlert(new Alert(
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

                _mediator.Register((Action<IMessage>)Action, entry.Value);
            }

            return true;
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
            for (var i = message.RemainingTicks; i > 0; i--)
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

            TryPlayClip(found ? countdownKey : $"countdown{message.RemainingTicks}");
        }

        private void HandlePieceSpawned(PieceSpawnedMessage message)
        {
            if (!string.IsNullOrEmpty(message.NextPiece))
                TryPlayClip(message.NextPiece[0].ToString().ToLower());
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
                case > 0 and < 4:
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
            else if (message.Y != 0 && message.WasHardDrop)
                TryPlayClip("harddrop");
        }

        private void PlayAsAnnouncer(string clipName)
        {
            if (_playedInThisUpdate.Contains(clipName) || RepressSfx)
                return;
            
            if (!SoundPackLoader.SoundEffects.TryGetValue(clipName, out var clip))
                if (!_defaultEffects.TryGetValue(clipName, out clip))
                {
                    AlertDisplayer.Instance.ShowAlert(new Alert(
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

        private void Play(string clipName)
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
                    AlertDisplayer.Instance.ShowAlert(new Alert(
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