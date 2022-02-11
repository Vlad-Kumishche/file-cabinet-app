using System.Collections;
using FileCabinetApp.Data;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Iterator for records in <see cref="MemoryIterator"/>.
    /// </summary>
    public class MemoryIterator : IEnumerable<FileCabinetRecord>
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
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            while (this.HasMore())
            {
                yield return this.GetCurrent();
                this.currentIndex++;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private FileCabinetRecord GetCurrent()
        {
            if (this.HasMore())
            {
                return this.records.ElementAt(this.currentIndex);
            }

            return new ();
        }

        private bool HasMore()
        {
            return this.currentIndex < this.records.Count();
        }
    }
}