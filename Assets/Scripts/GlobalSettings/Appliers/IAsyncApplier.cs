
/************************************
IAsyncApplier.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine.Events;

namespace UStacker.GlobalSettings.Appliers
{
    public interface IAsyncApplier
    {
        public UnityEvent LoadingStarted { get; }
        public UnityEvent LoadingFinished { get; }
        public string OngoingMessage { get; }
    }
}
/************************************
end IAsyncApplier.cs
*************************************/
