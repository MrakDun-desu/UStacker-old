using Blockstacker.Gameplay.Communication;

namespace Blockstacker.Gameplay.Stats
{
    public class LevellingSystemUtility : StatUtility
    {
        private readonly MediatorSO _mediator;
        
        public LevellingSystemUtility(MediatorSO mediator)
        {
            _mediator = mediator;
        }
        
        public void SetGravity(double gravity, double time)
        {
            _mediator.Send(new GravityChangedMessage{Gravity = gravity, Time = time});
        }
        
        public void SetLockDelay(double lockDelay, double time)
        {
            _mediator.Send(new LockDelayChangedMessage{LockDelay = lockDelay, Time = time});
        }
    }
}