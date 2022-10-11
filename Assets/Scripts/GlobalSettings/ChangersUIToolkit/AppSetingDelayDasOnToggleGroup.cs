using Blockstacker.GlobalSettings.Enums;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.ChangersUIToolkit
{
    public class AppSetingDelayDasOnToggleGroup : AppSettingToggleGroup<DelayDasOn>
    {
        public new class UxmlTraits : AppSettingToggleGroup<DelayDasOn>.UxmlTraits
        {
        }

        public new class UxmlFactory : UxmlFactory<AppSetingDelayDasOnToggleGroup, UxmlTraits>
        {
        }
    }
}