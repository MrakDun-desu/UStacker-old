using UnityEngine;

namespace UStacker.GameSettings.Enums
{
    public enum RandomizerType : byte
    {
        [Tooltip("Every seven pieces, one of each basic piece type spawns")]
        SevenBag = 0,
        [Tooltip("Seven Bag, but first bag will never start with S, Z or O piece")]
        Stride = 1,
        [Tooltip("Every fourteen pieces, two of each basic piece type spawn")]
        FourteenBag = 2,
        [Tooltip("Random with limited piece repetition protection")]
        Classic = 3,
        [Tooltip("Random. Pieces spawn in pairs")]
        Pairs = 4,
        [Tooltip("Totally random")]
        Random = 5,
        [Tooltip("User script will be used")]
        Custom = 6
    }
}