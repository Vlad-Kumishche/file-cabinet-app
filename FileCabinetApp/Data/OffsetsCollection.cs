using System.Collections;
using FileCabinetApp.Iterators;
using FileCabinetApp.Services;

namespace FileCabinetApp.Data
{
    /// <summary>
    /// Offsets collection.
    /// </summary>
    public sealed class OffsetsCollection : IEnumerable<FileCabinetRecord>
    {
        private readonly FileCabinetFilesystemService? service;
        private readonly IEnumerable<long> offsets;

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetsCollection"/> class.
        /// </summary>
        public OffsetsCollection()
        {
            this.service = null;
            this.offsets = new List<long>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetsCollection"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        /// <param name="offsets">Offsets.</param>
        public OffsetsCollection(FileCabinetFilesystemService service, IEnumerable<long> offsets)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.offsets = offsets ?? throw new ArgumentNullException(nameof(offsets));
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.service is null ? new FilesystemIterator() : new FilesystemIterator(this.service, this.offsets);

        /// <inheritdoc/>
        IEnumerator<FileCabinetRecord> IEnumerable<FileCabinetRecord>.GetEnumerator() => this.service is null ? new FilesystemIterator() : new FilesystemIterator(this.service, this.offsets);
    }
}
