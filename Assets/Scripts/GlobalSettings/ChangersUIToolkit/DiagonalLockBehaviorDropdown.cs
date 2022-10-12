using Blockstacker.GlobalSettings.Enums;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.ChangersUIToolkit
{
    public class DiagonalLockBehaviorDropdown : AppSettingEnumDropdown<DiagonalLockBehavior>
    {
        public new class UxmlTraits : AppSettingEnumDropdown<DiagonalLockBehavior>.UxmlTraits
        {
        }

        public new class UxmlFactory : UxmlFactory<DiagonalLockBehaviorDropdown, UxmlTraits>
        {
        }
    }
}