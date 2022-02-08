using System.Collections;
using FileCabinetApp.Iterators;

namespace FileCabinetApp.Data
{
    /// <summary>
    /// Offsets collection.
    /// </summary>
    public sealed class RecordsCollection : IEnumerable<FileCabinetRecord>
    {
        private readonly IEnumerable<FileCabinetRecord> records;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordsCollection"/> class.
        /// </summary>
        public RecordsCollection()
        {
            this.records = new List<FileCabinetRecord>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordsCollection"/> class.
        /// </summary>
        /// <param name="records">Offsets.</param>
        public RecordsCollection(IEnumerable<FileCabinetRecord> records)
        {
            this.records = records ?? throw new ArgumentNullException(nameof(records));
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => new MemoryIterator(this.records);

        /// <inheritdoc/>
        IEnumerator<FileCabinetRecord> IEnumerable<FileCabinetRecord>.GetEnumerator() => new MemoryIterator(this.records);
    }
}
