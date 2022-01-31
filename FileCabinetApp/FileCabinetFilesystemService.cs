using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Сlass provides a service for storing file cabinet records in filesystem and operations on them.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int RecordLength = 279;
        private const int MaxNameLength = 120;
        private readonly IRecordValidator validator;

        /// <summary>
        /// File stream to file.
        /// </summary>
        private FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">FileStream to specified file.</param>
        /// <param name="validator">Specified validation strategy.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream;
            this.validator = validator;
        }

        /// <summary>
        /// Gets length of file.
        /// </summary>
        /// <value>
        /// Length of file.
        /// </value>
        public int FileLength => (int)this.fileStream.Length;

        /// <summary>
        /// Gets counts of records.
        /// </summary>
        /// <value>
        /// Count of record.
        /// </value>
        public int RecordsCount => (int)this.fileStream.Length / RecordLength;

        /// <inheritdoc/>
        public int CreateRecord(RecordArgs recordToCreate)
        {
            this.validator.ValidateParameters(recordToCreate);

            var record = new FileCabinetRecord
            {
                Id = this.RecordsCount + 1,
                FirstName = recordToCreate.FirstName,
                LastName = recordToCreate.LastName,
                DateOfBirth = recordToCreate.DateOfBirth,
                Height = recordToCreate.Height,
                CashSavings = recordToCreate.CashSavings,
                FavoriteLetter = recordToCreate.FavoriteLetter,
            };

            // to avoid CS8604
            record.FirstName ??= string.Empty;
            record.LastName ??= string.Empty;

            var recordToFile = RecordToBytes(record);
            this.fileStream.Seek(0, SeekOrigin.End);
            this.fileStream.Write(recordToFile, 0, recordToFile.Length);
            this.fileStream.Flush();

            return record.Id;
        }

        /// <inheritdoc/>
        public void EditRecord(RecordArgs recordToEdit)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string sourceDate)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecordById(int id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            var recordBuffer = new byte[RecordLength];

            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < this.fileStream.Length; i += RecordLength)
            {
                this.fileStream.Read(recordBuffer, 0, RecordLength);
                records.Add(BytesToFileCabinetRecord(recordBuffer));
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        private static FileCabinetRecord BytesToFileCabinetRecord(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length < RecordLength)
            {
                throw new ArgumentException("Error. Record is corrupted.", nameof(bytes));
            }

            var record = new FileCabinetRecord();

            using (var memoryStream = new MemoryStream(bytes))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                short status = binaryReader.ReadInt16();
                record.Id = binaryReader.ReadInt32();

                record.FirstName = binaryReader.ReadString().Trim(' ');
                record.LastName = binaryReader.ReadString().Trim(' ');

                int year = binaryReader.ReadInt32();
                int month = binaryReader.ReadInt32();
                int day = binaryReader.ReadInt32();

                record.DateOfBirth = new DateTime(year, month, day);

                record.Height = binaryReader.ReadInt16();
                record.CashSavings = binaryReader.ReadDecimal();
                record.FavoriteLetter = binaryReader.ReadChar();
            }

            return record;
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return (int)(this.fileStream.Length / RecordLength);
        }

        /// <summary>
        /// Closes the stream.
        /// </summary>
        public void Close()
        {
            this.fileStream.Close();
        }

        private static byte[] RecordToBytes(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            var bytes = new byte[RecordLength];
            using (var memoryStream = new MemoryStream(bytes))
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                short status = 0;
                binaryWriter.Write(status);

                binaryWriter.Write(fileCabinetRecord.Id);

                // to avoid CS8604
                fileCabinetRecord.FirstName ??= string.Empty;
                fileCabinetRecord.LastName ??= string.Empty;

                binaryWriter.Write(fileCabinetRecord.FirstName.PadRight(MaxNameLength));
                binaryWriter.Write(fileCabinetRecord.LastName.PadRight(MaxNameLength));

                binaryWriter.Write(fileCabinetRecord.DateOfBirth.Year);
                binaryWriter.Write(fileCabinetRecord.DateOfBirth.Month);
                binaryWriter.Write(fileCabinetRecord.DateOfBirth.Day);
                binaryWriter.Write(fileCabinetRecord.Height);
                binaryWriter.Write(fileCabinetRecord.CashSavings);
                binaryWriter.Write(fileCabinetRecord.FavoriteLetter);
            }

            return bytes;
        }
    }
}
