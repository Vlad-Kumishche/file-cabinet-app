using FileCabinetApp.Data;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Iterator for records in <see cref="MemoryIterator"/>.
    /// </summary>
    public class MemoryIterator : IRecordIterator
    {
        private readonly IEnumerable<FileCabinetRecord> records;
        private int currentIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        public MemoryIterator()
        {
            this.records = new List<FileCabinetRecord>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="records">Records.</param>
        public MemoryIterator(IEnumerable<FileCabinetRecord> records)
        {
            this.records = records ?? throw new ArgumentNullException(nameof(records));
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetNext()
        {
            if (this.HasMore())
            {
                return this.records.ElementAt(this.currentIndex++);
            }

            return new FileCabinetRecord();
        }

        /// <inheritdoc/>
        public bool HasMore()
        {
            return this.currentIndex < this.records.Count();
        }
    }
}