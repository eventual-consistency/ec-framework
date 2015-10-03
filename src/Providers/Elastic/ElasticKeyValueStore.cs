using System;
using Elasticsearch.Net;
using Newtonsoft.Json;

namespace EventualConsistency.Providers.Elastic
{
    /// <summary>
    ///     The ElasticKeyValueStore is a simple Key/Value storage implementation
    ///     that lets us read and write from ElasticSearch.
    /// </summary>
    public class ElasticKeyValueStore<TResultType>
    {
        #region Constructor(s)

        /// <summary>
        ///     Initialize a new ElasticKeyValueStore
        /// </summary>
        /// <param name="elasticsearchClient">Elastic Search Client</param>
        /// <param name="indexName">Name of ElasticSearch index</param>
        public ElasticKeyValueStore(IElasticsearchClient elasticsearchClient, string indexName)
        {
            // Validate Arguments
            if (elasticsearchClient == null)
                throw new ArgumentNullException(nameof(elasticsearchClient));
            if (string.IsNullOrWhiteSpace(indexName))
                throw new ArgumentNullException(nameof(indexName));

            // Build instance
            Client = elasticsearchClient;
            IndexName = indexName;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Get the specified value from the dictionary
        /// </summary>
        /// <param name="itemKey">Item Key</param>
        /// <returns></returns>
        public TResultType Get(string itemKey)
        {
            // Validate arguments
            if (string.IsNullOrWhiteSpace(itemKey))
                throw new ArgumentNullException(nameof(itemKey));

            var result = Client.Get(IndexName, "_all", itemKey, null);

            if (result.Success)
            {
                var resultJson = result.ToString();
                var resultObject = JsonConvert.DeserializeObject<TResultType>(resultJson);
                return resultObject;
            }

            return default(TResultType);
        }

        /// <summary>
        ///     Put an item into the dictionary
        /// </summary>
        /// <param name="itemKey">Item Key</param>
        /// <param name="item">Item</param>
        /// <returns>True if put, false otherwise</returns>
        public bool Put(string itemKey, TResultType item)
        {
            // Validate arguments
            if (string.IsNullOrWhiteSpace(itemKey))
                throw new ArgumentNullException(nameof(itemKey));
            if (item == null)
                item = default(TResultType);

            var jsonData = JsonConvert.SerializeObject(item);

            var response = Client.Index(IndexName, typeof (TResultType).FullName, itemKey, jsonData, null);

            return response.Success;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     ElasticSearch Client
        /// </summary>
        protected IElasticsearchClient Client { get; }

        /// <summary>
        ///     ElasticSearch Index Name
        /// </summary>
        protected string IndexName { get; }

        #endregion
    }
}