﻿using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NodaMoney.Serialization.JsonNet
{
    /// <summary>Converts a instance of Money to and from JSON.</summary>
    public class MoneyJsonConverter : JsonConverter
    {
        /// <summary>Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON.</summary>
        /// <value><c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON; otherwise, <c>false</c>. </value>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <exception cref="ArgumentNullException">The value of 'writer', 'value' and 'serializer' cannot be null.</exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            Money money = (Money)value;

            writer.WriteStartObject();
            writer.WritePropertyName("amount");
            serializer.Serialize(writer, money.Amount.ToString(CultureInfo.InvariantCulture));
            writer.WritePropertyName("currency");
            serializer.Serialize(writer, money.Currency.Code);
            writer.WriteEndObject();
        }

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        /// <exception cref="ArgumentNullException">The value of 'reader' cannot be null.</exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (reader.TokenType == JsonToken.Null)
                return null;

            JObject jsonObject = JObject.Load(reader);
            var properties = jsonObject.Properties().ToList();

            var amountProperty = properties.SingleOrDefault(pr => string.Compare(pr.Name, "amount", StringComparison.OrdinalIgnoreCase) == 0);
            var currencyProperty = properties.SingleOrDefault(pr => string.Compare(pr.Name, "currency", StringComparison.OrdinalIgnoreCase) == 0);

            if (amountProperty == null)
                throw new ArgumentNullException("Ammount needs to be defined", "amount");

            if (currencyProperty == null)
                throw new ArgumentNullException("Currency needs to be defined", "currency");

            return new Money((decimal)amountProperty.Value, Currency.FromCode((string)currencyProperty.Value));
        }

        /// <summary>Determines whether this instance can convert the specified object type.</summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise,<c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Money);
        }
    }
}