using System;
using Newtonsoft.Json;
using UnityEngine;

namespace UStacker.Common.Converters
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);

            writer.WriteEndObject();
        }

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                throw new JsonReaderException();

            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonReaderException();

            var output = new Vector2();
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
                        var x = reader.ReadAsDouble();
                        if (x is null)
                            throw new JsonReaderException();

                        output.x = (float) x;
                        xSet = true;
                        break;
                    case "y":
                        var y = reader.ReadAsDouble();
                        if (y is null)
                            throw new JsonReaderException();

                        output.y = (float) y;
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