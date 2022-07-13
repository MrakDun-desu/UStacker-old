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
    public class CustomSoundEffectsPlayer : MonoBehaviour
    {
        [SerializeField] private MediatorSO _mediator;
        [SerializeField] private AudioClipCollection _defaultEffects = new();

        private AudioSource _audioSource;
        private readonly Lua _luaState = new();

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

                void Action(MidgameMessage message)
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
                        if (obj is not string value) return;
                        if (SoundPackLoader.SoundEffects.TryGetValue(value, out var clip))
                        {
                            _audioSource.PlayOneShot(clip);
                        }
                        else if (_defaultEffects.TryGetValue(value, out clip))
                        {
                            _audioSource.PlayOneShot(clip);
                        }
                    }
                }

                _mediator.Register((Action<MidgameMessage>) Action, entry.Value);
            }

        }

    }
}