using System;
using Newtonsoft.Json;
using UnityEngine;

namespace UStacker.Common.Converters
{
    public class Vector2IntConverter : JsonConverter<Vector2Int>
    {

        public override void WriteJson(JsonWriter writer, Vector2Int value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);

            writer.WriteEndObject();
        }

        public override Vector2Int ReadJson(JsonReader reader, Type objectType, Vector2Int existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                throw new JsonReaderException();

            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonReaderException();

            var output = new Vector2Int();
            var xSet = false;
            var ySet = false;

            reader.Read();
            while (!xSet || !ySet)
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    throw new JsonReaderException();

                switch (reader.Value as string)
                {
                    case "x":
                        var x = reader.ReadAsInt32();
                        if (x is null)
                            throw new JsonReaderException();

                        output.x = (int) x;
                        xSet = true;
                        break;
                    case "y":
                        var y = reader.ReadAsInt32();
                        if (y is null)
                            throw new JsonReaderException();

                        output.y = (int) y;
                        ySet = true;
                        break;
                    default:
                        throw new JsonReaderException();
                }

                reader.Read();
                if (reader.TokenType == JsonToken.EndObject)
                    return output;
            }

            throw new JsonReaderException();
        }
    }
}