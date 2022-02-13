using System.Collections;
using FileCabinetApp.Data;
using FileCabinetApp.Services;

namespace FileCabinetApp.Collections
{
    /// <summary>
    /// Collection for records in <see cref="FileCabinetFilesystemService"/>.
    /// </summary>
    public class FilesystemCollection : IEnumerable<FileCabinetRecord>
    {
        private readonly FileCabinetFilesystemService? service;
        private readonly IEnumerable<long> offsets;
        private int currentIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemCollection"/> class.
        /// </summary>
        public FilesystemCollection()
        {
            this.service = null;
            this.offsets = new List<long>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemCollection"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        /// <param name="offsets">Offsets.</param>
        public FilesystemCollection(FileCabinetFilesystemService service, IEnumerable<long> offsets)
        {
            this.service = service;
            this.offsets = offsets;
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