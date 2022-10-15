using Blockstacker.GlobalSettings.Enums;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.ChangersUIToolkit
{
    public class AppSettingDelayDasOnToggleGroup : AppSettingToggleGroup<DelayDasOn>
    {
        public new class UxmlTraits : AppSettingToggleGroup<DelayDasOn>.UxmlTraits
        {
        }

        public new class UxmlFactory : UxmlFactory<AppSettingDelayDasOnToggleGroup, UxmlTraits>
        {
        }
    }
}