using System.Collections;
using System.Collections.ObjectModel;
using FileCabinetApp.Data;

namespace FileCabinetApp.Collections
{
    /// <summary>
    /// Iterator for records in <see cref="MemoryCollection"/>.
    /// </summary>
    public class MemoryCollection : IEnumerable<FileCabinetRecord>
    {
        private readonly IEnumerable<FileCabinetRecord> records;
        private int currentIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCollection"/> class.
        /// </summary>
        public MemoryCollection()
        {
            this.records = new List<FileCabinetRecord>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCollection"/> class.
        /// </summary>
        /// <param name="records">Records.</param>
        public MemoryCollection(IEnumerable<FileCabinetRecord> records)
        {
            this.records = new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>(records));
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            this.currentIndex = 0;
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