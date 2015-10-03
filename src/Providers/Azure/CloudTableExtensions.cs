using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace EventualConsistency.Providers.Azure
{
    /// <summary>
    ///     Extensions for the Azure CloudTable class for table storage.
    /// </summary>
    public static class CloudTableExtensions
    {
        /// <summary>
        ///     Execute a table storage query asynchronously and resolve all rows, including
        ///     processing any continuation tokens.
        /// </summary>
        /// <typeparam name="T">Type of query</typeparam>
        /// <param name="table">CloudTable instance</param>
        /// <param name="query">Query to execute</param>
        /// <param name="ct">Cancellation token</param>
        /// <param name="onProgress">Progress handler</param>
        /// <returns>Result stream</returns>
        public static async Task<IList<T>> ExecuteQueryAsync<T>(this CloudTable table, TableQuery<T> query,
            CancellationToken ct = default(CancellationToken), Action<IList<T>> onProgress = null)
            where T : ITableEntity, new()
        {
            var items = new List<T>();
            TableContinuationToken token = null;
            do
            {
                var seg = await table.ExecuteQuerySegmentedAsync(query, token);
                token = seg.ContinuationToken;
                items.AddRange(seg);
                if (onProgress != null)
                    onProgress(items);
            } while (token != null && !ct.IsCancellationRequested);

            return items;
        }
    }
}