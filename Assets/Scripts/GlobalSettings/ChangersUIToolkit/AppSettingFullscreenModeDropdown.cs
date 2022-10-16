using Blockstacker.GlobalSettings.Enums;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.ChangersUIToolkit
{
    public class AppSettingFullscreenModeDropdown : AppSettingEnumDropdown<FullscreenMode>
    {
        public new class UxmlTraits : AppSettingEnumDropdown<FullscreenMode>.UxmlTraits
        {
        }

        public new class UxmlFactory : UxmlFactory<AppSettingFullscreenModeDropdown, UxmlTraits>
        {
        }
    }
}