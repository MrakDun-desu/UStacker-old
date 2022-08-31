using System;

namespace Blockstacker.GlobalSettings.StatCounting
{
    [Serializable]
    public class CustomStatCounterRecord : StatCounterRecord
    {
        public string SourceFile;
    }
}