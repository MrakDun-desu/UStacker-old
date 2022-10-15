using Blockstacker.GlobalSettings.Enums;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.ChangersUIToolkit
{
    public class AppSettingSimultaneousDasBehaviourDropdown : AppSettingEnumDropdown<SimultaneousDasBehavior>
    {
        public new class UxmlTraits : AppSettingEnumDropdown<SimultaneousDasBehavior>.UxmlTraits
        {
        }

        public new class UxmlFactory : UxmlFactory<AppSettingSimultaneousDasBehaviourDropdown, UxmlTraits>
        {
        }
    }
}