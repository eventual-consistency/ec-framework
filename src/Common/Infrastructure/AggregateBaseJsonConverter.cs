using System;
using System.Globalization;
using Newtonsoft.Json;

namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     The AggregateBaseJsonConverter is a JsonConverter that deals with the
    ///     aggregatebase type.
    /// </summary>
    public class AggregateBaseJsonConverter : JsonConverter
    {
        /// <summary>
        ///     Write JSON
        /// </summary>
        /// <param name="writer">JSON Writer</param>
        /// <param name="value">Current Instance</param>
        /// <param name="serializer">Serializer Reference</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var instance = (AggregateBase) value;

            writer.WriteStartObject();
            writer.WritePropertyName("Key");
            serializer.Serialize(writer, instance.Identity);
            writer.WritePropertyName("RevisionNumber");
            serializer.Serialize(writer, instance.RevisionNumber);
            writer.WritePropertyName("StateObject");
            serializer.Serialize(writer, instance.StateObject);

            writer.WriteEndObject();
        }

        /// <summary>
        ///     Read JSON from stream
        /// </summary>
        /// <param name="reader">JSON Reader</param>
        /// <param name="objectType">Object Type</param>
        /// <param name="existingValue">Existing Type</param>
        /// <param name="serializer">Serializer</param>
        /// <returns>Object instance</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            // Validate Arguments
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));
            if (serializer.ContractResolver == null)
                throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, ExceptionMessages.NoContractResolver, objectType.FullName));

            // Create an instance of the applicable type
            var contract = serializer.ContractResolver.ResolveContract(objectType);
            if (contract == null)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, ExceptionMessages.CouldNotResolveContract, objectType.FullName));

            var creator = contract.DefaultCreator;
            if (creator == null)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, ExceptionMessages.TypeMissingDefaultCreator, objectType.FullName));
            var instance = (AggregateBase)creator();

            serializer.Populate(reader, instance);

            // Return the type we've created
            return instance;
        }

        /// <summary>
        ///     Can convert specified object type to relevant type?
        /// </summary>
        /// <param name="objectType">Object Type</param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof (AggregateBase<,>).IsAssignableFrom(objectType);
        }
    }
}