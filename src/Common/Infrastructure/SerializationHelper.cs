using System;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     Helper methods for serialization
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        ///     Convert an object to a JSON notation.
        /// </summary>
        /// <param name="data">Object instance</param>
        /// <returns>JSON String</returns>
        public static string ToJson(this object data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        /// <summary>
        ///     Unpack a single event from it's JSON representation.
        /// </summary>
        /// <param name="eventType">Event Type</param>
        /// <param name="jsonData">JSON Representation</param>
        /// <param name="resolver">Type resolver</param>
        /// <returns>Reconstructed IEvent instance</returns>
        public static IEvent UnpackJsonEvent(this Type eventType, string jsonData, ITypeResolver resolver)
        {
            return JsonConvert.DeserializeObject(jsonData, eventType, new JsonSerializerSettings
            {
                ContractResolver = new TypeResolverContractResolver(resolver),
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            }) as IEvent;
        }

        /// <summary>
        ///     Unpack an instance from JSON
        /// </summary>
        /// <param name="jsonData">JSON Data</param>
        /// <typeparam name="TOutputType">Outout Type</typeparam>
        /// <param name="resolver">JSON Contract resolver</param>
        /// <returns>Deserialized object.</returns>
        public static TOutputType UnpackJson<TOutputType>(this string jsonData, ITypeResolver resolver)
        {
            // Validate Arguments
            if (string.IsNullOrWhiteSpace(jsonData))
                throw new ArgumentNullException(nameof(jsonData));
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            return JsonConvert.DeserializeObject<TOutputType>(jsonData, new JsonSerializerSettings
            {
                ContractResolver = new TypeResolverContractResolver(resolver),
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}