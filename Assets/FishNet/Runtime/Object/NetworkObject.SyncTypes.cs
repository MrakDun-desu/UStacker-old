using UnityEngine;

namespace FishNet.Object
{
    public sealed partial class NetworkObject : MonoBehaviour
    {
        /// <summary>
        ///     Writers dirty SyncTypes for all Networkbehaviours if their write tick has been met.
        /// </summary>
        internal void WriteDirtySyncTypes()
        {
            var nbs = NetworkBehaviours;
            var count = nbs.Length;
            for (var i = 0; i < count; i++)
            {
                //There was a null check here before, shouldn't be needed so it was removed.
                var nb = nbs[i];
                nb.WriteDirtySyncTypes(true, true);
                nb.WriteDirtySyncTypes(false, true);
            }
        }
    }
}