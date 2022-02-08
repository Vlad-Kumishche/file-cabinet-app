using System.Collections;
using FileCabinetApp.Data;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Iterator for records in <see cref="MemoryIterator"/>.
    /// </summary>
    public class MemoryIterator : IEnumerator<FileCabinetRecord>
    {
        private readonly IEnumerable<FileCabinetRecord> records;
        private int currentIndex;
        private bool disposed;

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
        public FileCabinetRecord Current => this.GetCurrent();

        /// <inheritdoc/>
        object IEnumerator.Current => this.Current;

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            if (this.HasMore())
            {
                this.currentIndex++;
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            this.currentIndex = 0;
        }

        /// <summary>
        /// Releasing resources.
        /// </summary>
        /// <param name="disposing">Whether the method call comes from a Dispose method (its value is true) or from a finalizer (its value is false).</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Reset();
            }

            this.disposed = true;
        }

        private FileCabinetRecord GetCurrent()
        {
            if (this.currentIndex < this.records.Count())
            {
                return this.records.ElementAt(this.currentIndex);
            }

            return new FileCabinetRecord();
        }

        private bool HasMore()
        {
            return this.currentIndex < this.records.Count() - 1;
        }
    }
}