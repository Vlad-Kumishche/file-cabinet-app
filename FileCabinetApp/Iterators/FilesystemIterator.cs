using System.Collections;
using FileCabinetApp.Data;
using FileCabinetApp.Services;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Iterator for records in <see cref="FileCabinetFilesystemService"/>.
    /// </summary>
    public class FilesystemIterator : IEnumerable<FileCabinetRecord>
    {
        private readonly FileCabinetFilesystemService? service;
        private readonly IEnumerable<long> offsets;
        private int currentIndex;

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
            if (this.HasMore() && this.service!.TryGetRecordByOffset(this.offsets.ElementAt(this.currentIndex), out var nextRecord))
            {
                return nextRecord;
            }

            return new ();
        }

        private bool HasMore()
        {
            return this.currentIndex < this.offsets.Count();
        }
    }
}