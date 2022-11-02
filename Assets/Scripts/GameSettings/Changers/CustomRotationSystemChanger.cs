﻿using Blockstacker.Common;

namespace Blockstacker.GameSettings.Changers
{
    public class CustomRotationSystemChanger : GameSettingFileChanger
    {
        protected override string DefaultEmptyPrompt => "No rotation system available";
        protected override string TargetDir => CustomizationPaths.RotationSystems;
    }
}