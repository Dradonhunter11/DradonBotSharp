using HypixelSharp.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HypixelSharp.CustomConverter
{
    class GameTypeConverter : JsonConverter<GameType>
    {
        public override GameType ReadJson(JsonReader reader, Type objectType, [AllowNull] GameType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string raw = reader.ReadAsString();
            return GameType.FromID(int.Parse(raw));
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] GameType value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }
    }
}
