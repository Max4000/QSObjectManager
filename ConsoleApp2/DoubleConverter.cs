using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyBookmark
{
    internal class DoubleConverter : JsonConverter
    {
        private const string POSITIVE_INFINITY = "Infinity";
        private const string NEGATIVE_INFINITY = "-Infinity";
        private const string NOT_A_NUMBER = "NaN";
        private readonly bool _canRead;
        private readonly bool _canWrite;

        public override bool CanRead => this._canRead;

        public override bool CanWrite => this._canWrite;

        public DoubleConverter(bool canRead = false, bool canWrite = false)
        {
            this._canRead = canRead;
            this._canWrite = canWrite;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!this.CanWrite)
                throw new NotSupportedException("DoubleConverter not configured to support write.");
            double d = (double)value;
            if (double.IsPositiveInfinity(d))
                writer.WriteValue("Infinity");
            else if (double.IsNegativeInfinity(d))
                writer.WriteValue("-Infinity");
            else if (double.IsNaN(d))
                writer.WriteValue("NaN");
            else
                writer.WriteRawValue(d.ToString("G17", (IFormatProvider)CultureInfo.InvariantCulture));
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (!this.CanRead)
                throw new NotSupportedException("DoubleConverter not configured to support read.");
            JToken jtoken = JToken.Load(reader);
            string str = Extensions.Value<string>((IEnumerable<JToken>)jtoken);
            if (str == "Infinity")
                return (object)double.PositiveInfinity;
            if (str == "-Infinity")
                return (object)double.NegativeInfinity;
            return str == "NaN" ? (object)double.NaN : (object)Extensions.Value<double>((IEnumerable<JToken>)jtoken);
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(double);
    }
}