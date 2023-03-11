using UnityEngine;
using UnityEngine.SceneManagement;
using UStacker.Multiplayer.Initialization;

namespace UStacker.Multiplayer
{
    public class MultiplayerGameOpener : MonoBehaviour
    {
        public void HostGame()
        {
            MultiplayerInitializer.InitType = MultiplayerInitType.Host;
            
            SceneManager.LoadScene("Scene_Game_Multiplayer");
        }

        public void PlayLocalGame()
        {
            MultiplayerInitializer.InitType = MultiplayerInitType.LocalClient;
            
            SceneManager.LoadScene("Scene_Game_Multiplayer");
        }
    }
}