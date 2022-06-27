using Blockstacker.Gameplay.Communication;
using Blockstacker.Music;
using UnityEngine;

namespace Blockstacker.Gameplay.SoundEffects
{
    [RequireComponent(typeof(AudioSource))]
    public class DefaultSoundEffectsPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClipCollection _defaultEffects = new();
        [SerializeField] private MediatorSO _mediator;

        private AudioSource _audioSource;
        private bool _wasCombo;
        private bool _wasBtb;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _mediator.Register<PiecePlacedMessage>(HandlePiecePlaced);
            _mediator.Register<PieceRotatedMessage>(HandlePieceRotated);
            _mediator.Register<PieceMovedMessage>(HandlePieceMoved);
            _mediator.Register<GameRestartedMessage>(_ =>
            {
                _wasBtb = false;
                _wasCombo = false;
            });
        }

        private void HandlePiecePlaced(PiecePlacedMessage midgameMessage)
        {
            if (midgameMessage.WasAllClear)
                TryPlayClip("allclear");

            switch (midgameMessage.LinesCleared)
            {
                case 0:
                    if (_wasCombo)
                    {
                        TryPlayClip("combobreak");
                        _wasCombo = false;
                    }

                    TryPlayClip("floor");
                    break;
                case > 0 and < 4:
                    if (midgameMessage.WasSpin || midgameMessage.WasSpinMini)
                    {
                        switch (midgameMessage.CurrentCombo)
                        {
                            case 0:
                                TryPlayClip("clearspin");
                                break;
                            case 1:
                                TryPlayClip("combo_1_power");
                                break;
                            case 2:
                                TryPlayClip("combo_2_power");
                                break;
                            case 3:
                                TryPlayClip("combo_3_power");
                                break;
                            case 4:
                                TryPlayClip("combo_4_power");
                                break;
                            case 5:
                                TryPlayClip("combo_5_power");
                                break;
                            case 6:
                                TryPlayClip("combo_6_power");
                                break;
                            case 7:
                                TryPlayClip("combo_7_power");
                                break;
                            case 8:
                                TryPlayClip("combo_8_power");
                                break;
                            case 9:
                                TryPlayClip("combo_9_power");
                                break;
                            case 10:
                                TryPlayClip("combo_10_power");
                                break;
                            case 11:
                                TryPlayClip("combo_11_power");
                                break;
                            case 12:
                                TryPlayClip("combo_12_power");
                                break;
                            case 13:
                                TryPlayClip("combo_13_power");
                                break;
                            case 14:
                                TryPlayClip("combo_14_power");
                                break;
                            case 15:
                                TryPlayClip("combo_15_power");
                                break;
                            case >= 16:
                                TryPlayClip("combo_16_power");
                                break;
                        }
                    }
                    else
                    {
                        if (_wasBtb)
                        {
                            TryPlayClip("btb_break");
                            _wasBtb = false;
                        }

                        switch (midgameMessage.CurrentCombo)
                        {
                            case 0:
                                TryPlayClip("clearline");
                                break;
                            case 1:
                                TryPlayClip("combo_1");
                                break;
                            case 2:
                                TryPlayClip("combo_2");
                                break;
                            case 3:
                                TryPlayClip("combo_3");
                                break;
                            case 4:
                                TryPlayClip("combo_4");
                                break;
                            case 5:
                                TryPlayClip("combo_5");
                                break;
                            case 6:
                                TryPlayClip("combo_6");
                                break;
                            case 7:
                                TryPlayClip("combo_7");
                                break;
                            case 8:
                                TryPlayClip("combo_8");
                                break;
                            case 9:
                                TryPlayClip("combo_9");
                                break;
                            case 10:
                                TryPlayClip("combo_10");
                                break;
                            case 11:
                                TryPlayClip("combo_11");
                                break;
                            case 12:
                                TryPlayClip("combo_12");
                                break;
                            case 13:
                                TryPlayClip("combo_13");
                                break;
                            case 14:
                                TryPlayClip("combo_14");
                                break;
                            case 15:
                                TryPlayClip("combo_15");
                                break;
                            case >= 16:
                                TryPlayClip("combo_16");
                                break;
                        }
                    }

                    break;
                case 4:
                    switch (midgameMessage.CurrentCombo)
                    {
                        case 0:
                            TryPlayClip("clearquad");
                            break;
                        case 1:
                            TryPlayClip("combo_1_power");
                            break;
                        case 2:
                            TryPlayClip("combo_2_power");
                            break;
                        case 3:
                            TryPlayClip("combo_3_power");
                            break;
                        case 4:
                            TryPlayClip("combo_4_power");
                            break;
                        case 5:
                            TryPlayClip("combo_5_power");
                            break;
                        case 6:
                            TryPlayClip("combo_6_power");
                            break;
                        case 7:
                            TryPlayClip("combo_7_power");
                            break;
                        case 8:
                            TryPlayClip("combo_8_power");
                            break;
                        case 9:
                            TryPlayClip("combo_9_power");
                            break;
                        case 10:
                            TryPlayClip("combo_10_power");
                            break;
                        case 11:
                            TryPlayClip("combo_11_power");
                            break;
                        case 12:
                            TryPlayClip("combo_12_power");
                            break;
                        case 13:
                            TryPlayClip("combo_13_power");
                            break;
                        case 14:
                            TryPlayClip("combo_14_power");
                            break;
                        case 15:
                            TryPlayClip("combo_15_power");
                            break;
                        case >= 16:
                            TryPlayClip("combo_16_power");
                            break;
                    }

                    break;
            }

            if (midgameMessage.CurrentCombo > 0) _wasCombo = true;
            if (midgameMessage.CurrentBackToBack > 0) _wasBtb = true;
        }

        private void HandlePieceRotated(PieceRotatedMessage midgameMessage)
        {
            if (midgameMessage.WasSpin || midgameMessage.WasSpinMini)
                TryPlayClip("spin");
            else
                TryPlayClip("rotate");
        }

        private void HandlePieceMoved(PieceMovedMessage midgameMessage)
        {
            if (midgameMessage.X != 0)
                TryPlayClip("move");
            else if (midgameMessage.Y != 0 && midgameMessage.WasSoftDrop)
                TryPlayClip("softdrop");
            else if (midgameMessage.Y != 0 && midgameMessage.WasHardDrop)
                TryPlayClip("harddrop");
        }

        private void TryPlayClip(string clipName)
        {
            if (SoundPackLoader.SoundEffects.TryGetValue(clipName, out var clip))
                _audioSource.PlayOneShot(clip);
            else if (_defaultEffects.TryGetValue(clipName, out clip))
                _audioSource.PlayOneShot(clip);
            else
                Debug.Log("Clip not found!");
        }
    }
}