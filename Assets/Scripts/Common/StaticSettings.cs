using Blockstacker.Common.Converters;
using Newtonsoft.Json;

namespace Blockstacker.Common
{
    public static class StaticSettings
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
#if UNITY_EDITOR
            Formatting = Formatting.Indented,
#endif
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = new JsonConverter[]
            {
                new Vector2Converter(),
                new Vector2IntConverter()
            },
            Error = (_, args) =>
            {
                args.ErrorContext.Handled = true;
            }
        };

        public const string WikiUrl = "https://github.com/MrakDun-desu/BlockstackerDocs/";

    }
}