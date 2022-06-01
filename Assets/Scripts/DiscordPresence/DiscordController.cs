using Blockstacker.Common;
using UnityEngine;

namespace Blockstacker.DiscordPresence
{
    public class DiscordController : MonoSingleton
    {
        private static long ApplicationID => 953585016779202580;
        private Discord.Discord discord;

        private void Update() => discord?.RunCallbacks();

        private void OnDisable() => DisconnectFromDiscord();

        private void OnApplicationQuit() => DisconnectFromDiscord();

        public void ConnectToDiscord()
        {
            discord = new Discord.Discord(ApplicationID, (ulong)Discord.CreateFlags.Default);
            var activityManager = discord.GetActivityManager();
            var activity = new Discord.Activity
            {
                State = "Still Testing",
                Details = "Imagine you see me stacking blocks here"
            };
            activityManager.UpdateActivity(activity, res =>
            {
            });
        }

        public void DisconnectFromDiscord()
        {
            discord?.Dispose();
            discord = null;
        }

    }
}