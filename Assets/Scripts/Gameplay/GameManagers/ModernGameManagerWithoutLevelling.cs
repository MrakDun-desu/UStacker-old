using Blockstacker.Gameplay.Communication;
using UnityEngine;

namespace Blockstacker.Gameplay.GameManagers
{
    public class ModernGameManagerWithoutLevelling : MonoBehaviour, IGameManager
    {
        private const double GRAVITY = 1d / 60d;

        private long _currentScore;

        private MediatorSO _mediator;

        public void Initialize(string _, MediatorSO mediator)
        {
            _mediator = mediator;

            _mediator.Register<PiecePlacedMessage>(HandlePiecePlaced);
            _mediator.Register<PieceMovedMessage>(HandlePieceMoved);
            _mediator.Register<GameStartedMessage>(_ => ResetGameManager());
            _mediator.Register<GameRestartedMessage>(_ => ResetGameManager());
        }

        private void ResetGameManager()
        {
            _currentScore = 0;
            _mediator.Send(new ScoreChangedMessage(0, 0));
            _mediator.Send(new GravityChangedMessage(GRAVITY, 0));
            _mediator.Send(new LevelChangedMessage(string.Empty, 0));
            _mediator.Send(new LevelUpConditionChangedMessage(0, 0, 0, "None"));
        }

        private void HandlePiecePlaced(PiecePlacedMessage message)
        {
            long scoreAddition;
            if (message.WasSpin)
            {
                scoreAddition = ((int) message.LinesCleared + 1) * 400;
            }
            else if (message.WasSpinMini)
            {
                scoreAddition = message.LinesCleared switch
                {
                    0 => 100,
                    1 => 200,
                    2 => 400,
                    var amount => (int) amount * 300
                };
            }
            else
            {
                scoreAddition = message.LinesCleared switch
                {
                    0 => 0,
                    1 => 100,
                    2 => 300,
                    3 => 500,
                    4 => 800,
                    var amount => (int) amount * 200
                };
            }

            scoreAddition += (int) message.CurrentCombo * 50;
            if (message.WasAllClear)
                scoreAddition += 3000;

            if (scoreAddition == 0)
                return;

            const float backToBackMultiplier = 1.5f;
            if (message.CurrentBackToBack >= 1 && message.LinesCleared > 0)
                scoreAddition = (int) (scoreAddition * backToBackMultiplier);

            _currentScore += scoreAddition;
            _mediator.Send(new ScoreChangedMessage(_currentScore, message.Time));
        }

        private void HandlePieceMoved(PieceMovedMessage message)
        {
            var scoreAddition = message switch
            {
                {WasHardDrop: true, Y: var y} => -2 * y,
                {WasSoftDrop: true, Y: var y} => -y,
                _ => 0
            };

            if (scoreAddition == 0) return;
            _currentScore += scoreAddition;
            _mediator.Send(new ScoreChangedMessage(_currentScore, message.Time));
        }
    }
}
