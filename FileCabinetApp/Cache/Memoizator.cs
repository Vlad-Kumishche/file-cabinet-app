using System.Text;
using FileCabinetApp.Data;
using FileCabinetApp.Services;

namespace FileCabinetApp.Cache
{
    /// <summary>
    /// Performs memoization.
    /// </summary>
    public class Memoizator
    {
        private readonly IFileCabinetService service;
        private readonly Dictionary<string, IEnumerable<FileCabinetRecord>> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="Memoizator"/> class.
        /// </summary>
        /// <param name="service">Used service.</param>
        public Memoizator(IFileCabinetService service)
        {
            this.service = service;
            this.cache = new ();
        }

        /// <summary>
        /// Returns a selection of records based on search parameters. If the selection is already cached, returns the cached collection.
        /// </summary>
        /// <param name="searchOptions">A set of parameters to select the records.</param>
        /// <param name="logicalOperator">Logical operator.</param>
        /// <returns>Selection of records.</returns>
        public IEnumerable<FileCabinetRecord> SelectByOptions(List<KeyValuePair<string, string>> searchOptions, string logicalOperator)
        {
            var key = GenerateKey(searchOptions, logicalOperator);
            if (this.cache.TryGetValue(key, out var records))
            {
                return records;
            }

            records = this.service.SelectByOptions(searchOptions, logicalOperator);
            return records;
        }

        /// <summary>
        /// Tries to get records.
        /// </summary>
        /// <param name="searchOptions">A set of parameters to select the records.</param>
        /// <param name="logicalOperator">Logical operator.</param>
        /// <param name="records">Selected records.</param>
        /// <returns>Is successful.</returns>
        public bool TryGetRecords(List<KeyValuePair<string, string>> searchOptions, string logicalOperator, out IEnumerable<FileCabinetRecord> records)
        {
            var key = GenerateKey(searchOptions, logicalOperator);
            if (this.cache.TryGetValue(key, out var result))
            {
                records = result;
                return true;
            }

            records = new List<FileCabinetRecord>();
            return false;
        }

        /// <summary>
        /// Adds result of selection to cache.
        /// </summary>
        /// <param name="searchOptions">A set of parameters to select the records.</param>
        /// <param name="logicalOperator">Logical operator.</param>
        /// <param name="records">Selected records.</param>
        public void AddResult(List<KeyValuePair<string, string>> searchOptions, string logicalOperator, IEnumerable<FileCabinetRecord> records)
        {
            var key = GenerateKey(searchOptions, logicalOperator);
            this.cache.Add(key, records);
        }

        /// <summary>
        /// Cleares the cache.
        /// </summary>
        public void Clear()
        {
            this.cache.Clear();
        }

        private static string GenerateKey(List<KeyValuePair<string, string>> searchOptions, string logicalOperator)
        {
            const char separator = ',';
            var generatedKey = new StringBuilder();
            generatedKey.Append(separator);
            foreach (var keyValuePair in searchOptions)
            {
                generatedKey.Append(keyValuePair.Key);
                generatedKey.Append(separator);
                generatedKey.Append(keyValuePair.Value);
                generatedKey.Append(separator);
            }

            generatedKey.Append(logicalOperator);
            return generatedKey.ToString();
        }
    }
}
