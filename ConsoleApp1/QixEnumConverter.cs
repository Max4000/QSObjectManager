using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Qlik.Engine;

namespace ConsoleApp1
{
    public class QixEnumConverter : JsonConverter
    {
        private static readonly StringEnumConverter BaseConverter = new();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                Enum @enum = (Enum)value;
                string enumName = @enum.ToString("G");
                if (value.GetType().GetMembers().Where((Func<MemberInfo, bool>)(info => info.Name.Equals(enumName))).SelectMany((Func<MemberInfo, IEnumerable<object>>)(info => info.GetCustomAttributes(typeof(QixNameAttribute), true))).FirstOrDefault() is QixNameAttribute qixNameAttribute2)
                    writer.WriteValue(qixNameAttribute2.Name);
                else
                    writer.WriteValue(@enum);
            }
        }

        public override object ReadJson(
          JsonReader reader,
          Type objectType,
          object existingValue,
          JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string enumText = reader.Value.ToString();
                bool flag = QixEnumConverter.IsNullableType(objectType);
                if (string.IsNullOrWhiteSpace(enumText) & flag)
                    return null;
                Type enumType = flag ? Nullable.GetUnderlyingType(objectType) : objectType;
                if (enumType != null)
                {
                    MemberInfo memberInfo = enumType.GetMembers().FirstOrDefault((Func<MemberInfo, bool>)(info => Enumerable.OfType<QixNameAttribute>(info.GetCustomAttributes(typeof(QixNameAttribute), true)).Any((Func<QixNameAttribute, bool>)(attribute => attribute.Name.Equals(enumText)))));
                    if (memberInfo != null)
                        return Enum.Parse(enumType, memberInfo.Name);
                }
            }
            else if (reader.TokenType == JsonToken.Integer)
            {
                object obj = reader.Value;
                if (obj is long l)
                    obj = (int)l;
                if (objectType.IsEnumDefined(obj))
                    return Enum.Parse(objectType, objectType.GetEnumName(obj)!);
            }
            return QixEnumConverter.BaseConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override bool CanConvert(Type objectType) => objectType.IsEnum;

        private static bool IsNullableType(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
}