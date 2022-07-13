using Blockstacker.Gameplay.Communication;
using Blockstacker.GlobalSettings.Music;
using UnityEngine;

namespace Blockstacker.Gameplay.SoundEffects
{
    [RequireComponent(typeof(AudioSource))]
    public class DefaultSoundEffectsPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClipCollection _defaultEffects = new();
        [SerializeField] private MediatorSO _mediator;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
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

        private void HandleHoldUsed(HoldUsedMessage obj)
        {
            if (obj.WasSuccessful)
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
            if (message.NextPiece.EndsWith("Piece") && message.NextPiece.Length == 6)
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