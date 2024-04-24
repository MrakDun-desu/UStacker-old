
/************************************
CommunityLinkOpener.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;

namespace UStacker.Common.UI
{
    public class CommunityLinkOpener : MonoBehaviour
    {
        public void OpenDiscord()
        {
            Application.OpenURL("https://discord.gg/2KQVs9TQ9M");
        }

        public void OpenItchIo()
        {
            Application.OpenURL("https://mrakdun-desu.itch.io/UStacker");
        }
    }
}
/************************************
end CommunityLinkOpener.cs
*************************************/
