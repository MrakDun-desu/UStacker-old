﻿using Blockstacker.Common;

namespace Blockstacker.GameSettings.Changers
{
    public class CustomGarbageGeneratorChanger : GameSettingFileChanger
    {
        protected override string TargetDir => CustomizationPaths.GarbageGenerators;
        protected override string DefaultEmptyPrompt => "No garbage generator available";
    }
}