// -----------------------------------------------------------------------
// <copyright file="UnixEpochDateTimeConverter.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Persona
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    internal class UnixEpochDateTimeConverter : DateTimeConverterBase
    {
        private static readonly DateTimeOffset Epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This is not exposed publicly.")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "This is not exposed publicly.")]
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var nullable = objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>);
            var t = nullable
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            if (reader.TokenType == JsonToken.Null)
            {
                if (!nullable)
                {
                    throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Cannot convert null value to {0}.", objectType));
                }

                return null;
            }
            else if (reader.TokenType == JsonToken.Date)
            {
                if (t == typeof(DateTimeOffset))
                {
                    return reader.Value is DateTimeOffset ? reader.Value : new DateTimeOffset((DateTime)reader.Value);
                }

                return reader.Value;
            }
            else if (reader.TokenType != JsonToken.Float && reader.TokenType != JsonToken.Integer)
            {
                throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Unexpected token parsing date. Expected Float or Integer, got {0}.", reader.TokenType));
            }
            else
            {
                var date = Epoch.AddMilliseconds((double)(dynamic)reader.Value);

                return t == typeof(DateTimeOffset)
                    ? date
                    : date.UtcDateTime;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This is not exposed publicly.")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "This is not exposed publicly.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DateTime", Justification = "This is spelled correctly.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DateTimeOffset", Justification = "This is spelled correctly.")]
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                writer.WriteValue((new DateTimeOffset((DateTime)value) - Epoch).TotalMilliseconds);
            }
            else if (value is DateTimeOffset)
            {
                writer.WriteValue(((DateTimeOffset)value - Epoch).TotalMilliseconds);
            }
            else
            {
                throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Unexpected value when converting date. Expected DateTime or DateTimeOffset, got {0}.", value.GetType()));
            }
        }
    }
}
