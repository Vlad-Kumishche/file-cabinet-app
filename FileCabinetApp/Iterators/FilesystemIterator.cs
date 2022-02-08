using System.Collections;
using FileCabinetApp.Data;
using FileCabinetApp.Services;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Iterator for records in <see cref="FileCabinetFilesystemService"/>.
    /// </summary>
    public class FilesystemIterator : IEnumerator<FileCabinetRecord>
    {
        private readonly FileCabinetFilesystemService? service;
        private readonly IEnumerable<long> offsets;
        private int currentIndex;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        public FilesystemIterator()
        {
            this.service = null;
            this.offsets = new List<long>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        /// <param name="offsets">Offsets.</param>
        public FilesystemIterator(FileCabinetFilesystemService service, IEnumerable<long> offsets)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.offsets = offsets ?? throw new ArgumentNullException(nameof(offsets));
        }

        /// <inheritdoc/>
        public FileCabinetRecord Current => this.GetCurrent();

        /// <inheritdoc/>
        object IEnumerator.Current => this.Current;

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

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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
            if (this.currentIndex < this.offsets.Count() && this.service!.TryGetRecordByOffset(this.offsets.ElementAt(this.currentIndex++), out var nextRecord))
            {
                return nextRecord;
            }

            return new FileCabinetRecord();
        }

        private bool HasMore()
        {
            return this.currentIndex < this.offsets.Count() - 1;
        }
    }
}