using Blockstacker.DiscordPresence;

namespace Blockstacker.Settings.Appliers
{
    public class DiscordActivityApplier : SettingApplierBase
    {
        DiscordController _dcController;

        protected override void Awake()
        {
            base.Awake();
            _dcController = FindObjectOfType<DiscordController>();
        }

        public override void OnSettingChanged()
        {
            if (_dcController == null) return;

            if (AppSettings.Other.UseDiscordRichPresence) {
                _dcController.Start();
            }
            else {
                _dcController.Stop();
            }
        }
    }
}