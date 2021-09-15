// Decompiled with JetBrains decompiler
// Type: Qlik.Engine.Communication.Converters.CharToIntConverter
// Assembly: Qlik.Engine, Version=15.4.2.0, Culture=neutral, PublicKeyToken=1a848309662c81e5
// MVID: 7A78A7A1-A351-49F4-A552-0DE92834559B
// Assembly location: C:\Users\Anatoliy\.nuget\packages\qliksense.netsdk\15.4.2\ref\netcoreapp2.1\Qlik.Engine.dll

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ObjectsForWorkWithQSEngine.Converters
{
    internal class CharToIntConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => writer.WriteValue((int)(char)value);

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            return (char)(ushort)JToken.Load(reader).Value<int>();
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(char);
    }
}