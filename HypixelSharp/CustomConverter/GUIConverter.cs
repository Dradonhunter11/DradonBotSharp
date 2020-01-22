using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HypixelSharp.CustomConverter
{
    class GUIConverter : JsonConverter<Guid>
    {
        public override Guid ReadJson(JsonReader reader, Type objectType, [AllowNull] Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            String guid = reader.ReadAsString();
            return (guid.Contains("-")) ? Guid.Parse(guid) 
                : Guid.Parse(guid.Substring(0, 8) + "-" + guid.Substring(8, 12) + "-" + guid.Substring(12, 16) + "-" + guid.Substring(16, 20) + "-" + guid.Substring(20, 32));
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] Guid value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
