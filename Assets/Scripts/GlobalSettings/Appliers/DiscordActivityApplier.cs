using Blockstacker.DiscordPresence;

namespace Blockstacker.GlobalSettings.Appliers
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