
/************************************
DiscordController.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using Discord;
using UnityEngine.SceneManagement;
using UStacker.Common;
using UStacker.Common.Alerts;

namespace UStacker.DiscordPresence
{
    public class DiscordController : MonoSingleton<DiscordController>
    {
        private Discord.Discord discord;
        private static long ApplicationID => 953585016779202580;

        private void Update()
        {
            try
            {
                discord?.RunCallbacks();
            }
            catch
            {
                DisconnectFromDiscord();
            }
        }

        private void OnDisable()
        {
            DisconnectFromDiscord();
        }

        private void OnApplicationQuit()
        {
            DisconnectFromDiscord();
        }

        public void ConnectToDiscord()
        {
            try
            {
                discord = new Discord.Discord(ApplicationID, (ulong) CreateFlags.NoRequireDiscord);
                SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
            }
            catch
            {
                discord = null;
            }
        }

        private void SceneManagerOnsceneLoaded(Scene scene, LoadSceneMode _)
        {
            if (discord is null)
                return;

            var activity = new Activity
            {
                State = scene.name switch
                {
                    "Scene_Menu_Main" => "In main menu",
                    "Scene_Menu_GameSettings_Singleplayer" => "Deciding how to play a game",
                    "Scene_Menu_GameSettings_Custom" => "Creating a custom game",
                    "Scene_Game_Singleplayer" => "In a singleplayer game",
                    "Scene_Game_Multiplayer" => "In a multiplayer game",
                    _ => string.Empty
                },
                ApplicationId = ApplicationID,
                Timestamps = new ActivityTimestamps {Start = DateTimeOffset.Now.ToUnixTimeSeconds()}
            };
            discord.GetActivityManager().UpdateActivity(activity, ActivityUpdatedCallback);
        }

        private void ActivityUpdatedCallback(Result result)
        {
            if (result == Result.Ok) return;

            AlertDisplayer.ShowAlert(new Alert("Couldn't connect to Discord!", "Disabling rich presence.",
                AlertType.Info));
            DisconnectFromDiscord();
        }


        public void DisconnectFromDiscord()
        {
            SceneManager.sceneLoaded -= SceneManagerOnsceneLoaded;
            discord?.Dispose();
            discord = null;
        }
    }
}
/************************************
end DiscordController.cs
*************************************/
