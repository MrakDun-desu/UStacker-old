using UStacker.Common.Converters;
using Newtonsoft.Json;

namespace UStacker.Common
{
    public static class StaticSettings
    {

        public const string WikiUrl = "https://github.com/MrakDun-desu/UStackerDocs/";
        public static readonly JsonSerializerSettings DefaultSerializerSettings = new()
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
            Error = (_, args) => { args.ErrorContext.Handled = true; }
        };

        public static readonly JsonSerializerSettings ReplaySerializerSettings = new()
        {
#if UNITY_EDITOR
            Formatting = Formatting.Indented,
#endif
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = new JsonConverter[]
            {
                new Vector2Converter(),
                new Vector2IntConverter()
            }
        };
    }
}