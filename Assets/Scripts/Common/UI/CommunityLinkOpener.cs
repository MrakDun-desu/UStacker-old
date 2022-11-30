using UnityEngine;

namespace Blockstacker.Common.UI
{
    public class CommunityLinkOpener : MonoBehaviour
    {
        public void OpenDiscord()
        {
            Application.OpenURL("https://discord.gg/2KQVs9TQ9M");
        }

        public void OpenItchIo()
        {
            Application.OpenURL("https://mrakdun-desu.itch.io/blockstacker");
        }
    }
}