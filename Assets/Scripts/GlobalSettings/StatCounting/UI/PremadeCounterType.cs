using System;

namespace Blockstacker.GlobalSettings.StatCounting.UI
{
    [Serializable]
    public class PremadeCounterType
    {
        public string Name;
        public StatCounterType Type;
        public StatCounterSO StatCounterSo;
    }
}