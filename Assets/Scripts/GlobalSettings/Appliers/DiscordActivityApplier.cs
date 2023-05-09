
/************************************
DiscordActivityApplier.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UStacker.DiscordPresence;

namespace UStacker.GlobalSettings.Appliers
{
    public class DiscordActivityApplier : SettingApplierBase
    {
        private DiscordController _dcController;

        protected override void Awake()
        {
            base.Awake();
            _dcController = FindObjectOfType<DiscordController>();
        }

        public override void OnSettingChanged()
        {
            if (_dcController == null) return;

            if (AppSettings.Other.UseDiscordRichPresence)
                _dcController.ConnectToDiscord();
            else
                _dcController.DisconnectFromDiscord();
        }
    }
}
/************************************
end DiscordActivityApplier.cs
*************************************/
