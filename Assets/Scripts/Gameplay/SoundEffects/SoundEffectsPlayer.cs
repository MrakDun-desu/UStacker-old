using System;
using System.Collections.Generic;
using Blockstacker.Gameplay.Communication;
using Blockstacker.GlobalSettings.Music;
using NLua;
using NLua.Exceptions;
using UnityEngine;

namespace Blockstacker.Gameplay.SoundEffects
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffectsPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClipCollection _defaultEffects = new();
        [SerializeField] private MediatorSO _mediator;

        private AudioSource _audioSource;
        private Lua _luaState;
        
        private readonly Dictionary<string, Type> RegisterableEvents = new()
        {
            {
                "PieceSpawned", typeof(PieceSpawnedMessage)
            },
            {
                "CountdownTicked", typeof(CountdownTickedMessage)
            },
            {
                "PiecePlaced", typeof(PiecePlacedMessage)
            },
            {
                "PieceMoved", typeof(PieceMovedMessage)
            },
            {
                "HoldUsed", typeof(HoldUsedMessage)
            },
            {
                "InputAction", typeof(InputActionMessage)
            },
            {
                "PieceRotated", typeof(PieceRotatedMessage)
            },
            {
                "GameLost", typeof(GameLostMessage)
            },
            {
                "GameEnded", typeof(GameEndedMessage)
            },
            {
                "GamePaused", typeof(GamePausedMessage)
            },
            {
                "GameResumed", typeof(GameResumedMessage)
            },
            {
                "GameStarted", typeof(GameResumedMessage)
            },
        };

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (string.IsNullOrEmpty(SoundPackLoader.SoundEffectsScript))
                RegisterDefaultFunctions();
            else
                RegisterCustomFunctions();
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

        private void RegisterCustomFunctions()
        {
            
            LuaTable events = null;
            try
            {
                if (_luaState.DoString(SoundPackLoader.SoundEffectsScript)[0] is LuaTable eventTable)
                {
                    events = eventTable;
                }
            }
            catch (LuaException ex)
            {
                Debug.Log(ex.Message);
            }

            if (events is null) return;
            
            foreach (var entry in RegisterableEvents)
            {
                if (events[entry.Key] is not LuaFunction function) continue;

                void Action(Message message)
                {
                    object[] output = null;
                    try
                    {
                        output = function.Call(message);
                    }
                    catch (LuaException ex)
                    {
                        Debug.Log(ex.Message);
                    }

                    if (output is null) return;
                    
                    foreach (var obj in output)
                    {
                        if (obj is not string clipName) return;
                        
                        TryPlayClip(clipName);    
                    }
                }

                _mediator.Register((Action<Message>) Action, entry.Value);
            }
        }

        private void HandleHoldUsed(HoldUsedMessage message)
        {
            if (message.WasSuccessful)
                TryPlayClip("hold");
        }

        private void HandleCountdownTicked(CountdownTickedMessage message)
        {
            switch (message.RemainingTicks)
            {
                case < 4:
                    TryPlayClip($"countdown{message.RemainingTicks + 1}");
                    break;
                case >= 4:
                    TryPlayClip("countdown5");
                    break;
            }
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
                    if (message.BrokenCombo)
                    {
                        TryPlayClip("combobreak");
                    }

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

        private void TryPlayClip(string clipName)
        {
            if (SoundPackLoader.SoundEffects.TryGetValue(clipName, out var clip))
                _audioSource.PlayOneShot(clip);
            else if (_defaultEffects.TryGetValue(clipName, out clip))
                _audioSource.PlayOneShot(clip);
            else
                Debug.Log($"Clip {clipName} not found!");
        }
    }
}