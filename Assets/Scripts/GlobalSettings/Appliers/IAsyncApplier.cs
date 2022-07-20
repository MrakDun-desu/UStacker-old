using UnityEngine.Events;

namespace Blockstacker.GlobalSettings.Appliers
{
    public interface IAsyncApplier
    {
        public UnityEvent LoadingStarted { get; }
        public UnityEvent LoadingFinished { get; }
        public string OngoingMessage { get; }
    }
}