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
        private const int RecordSize = 279;
        private const int MaxNameLength = 120;
        private static readonly ReadOnlyCollection<FileCabinetRecord> EmptyRecordReadOnlyCollection = new List<FileCabinetRecord>().AsReadOnly();
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
        public int RecordsCount => (int)this.fileStream.Length / RecordSize;

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
            this.validator.ValidateParameters(recordToEdit);

            var record = new FileCabinetRecord
            {
                Id = recordToEdit.Id,
                FirstName = recordToEdit.FirstName,
                LastName = recordToEdit.LastName,
                DateOfBirth = recordToEdit.DateOfBirth,
                Height = recordToEdit.Height,
                CashSavings = recordToEdit.CashSavings,
                FavoriteLetter = recordToEdit.FavoriteLetter,
            };

            var recordNumberToChange = -1;
            var recordBuffer = new byte[RecordSize];

            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 0, recordNumber = 0; i < this.fileStream.Length; i += RecordSize, recordNumber++)
            {
                this.fileStream.Read(recordBuffer, 0, RecordSize);
                var currendRecord = BytesToFileCabinetRecord(recordBuffer);

                if (currendRecord.Id == record.Id)
                {
                    recordNumberToChange = recordNumber;
                    break;
                }
            }

            if (recordNumberToChange >= 0)
            {
                var bytesOfRecordParameters = FileCabinetRecordToBytes(record);
                this.fileStream.Seek(recordNumberToChange * RecordSize, SeekOrigin.Begin);
                this.fileStream.Write(bytesOfRecordParameters, 0, bytesOfRecordParameters.Length);
                this.fileStream.Flush();
            }
            else
            {
                throw new ArgumentException($"There is no record with {nameof(record.Id)} == {record.Id}", nameof(recordToEdit));
            }
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> recorecordsFound = new List<FileCabinetRecord>();
            foreach (var record in this.GetRecords())
            {
                if (string.Equals(record.FirstName, firstName, StringComparison.OrdinalIgnoreCase))
                {
                    recorecordsFound.Add(record);
                }
            }

            var records = new ReadOnlyCollection<FileCabinetRecord>(recorecordsFound);
            return records;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<FileCabinetRecord> recorecordsFound = new List<FileCabinetRecord>();
            foreach (var record in this.GetRecords())
            {
                if (string.Equals(record.LastName, lastName, StringComparison.OrdinalIgnoreCase))
                {
                    recorecordsFound.Add(record);
                }
            }

            var records = new ReadOnlyCollection<FileCabinetRecord>(recorecordsFound);
            return records;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string sourceDate)
        {
            if (!DateTime.TryParse(sourceDate, out var dateOfBirth))
            {
                return EmptyRecordReadOnlyCollection;
            }

            List<FileCabinetRecord> recorecordsFound = new List<FileCabinetRecord>();
            foreach (var record in this.GetRecords())
            {
                if (record.DateOfBirth == dateOfBirth)
                {
                    recorecordsFound.Add(record);
                }
            }

            var records = new ReadOnlyCollection<FileCabinetRecord>(recorecordsFound);
            return records;
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecordById(int id)
        {
            var recordNumberToChange = -1;
            var recordBuffer = new byte[RecordSize];
            FileCabinetRecord searchedRecord = new FileCabinetRecord();

            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 0, recordNumber = 0; i < this.fileStream.Length; i += RecordSize, recordNumber++)
            {
                this.fileStream.Read(recordBuffer, 0, RecordSize);
                searchedRecord = BytesToFileCabinetRecord(recordBuffer);

                if (searchedRecord.Id == id)
                {
                    recordNumberToChange = recordNumber;
                    break;
                }
            }

            if (recordNumberToChange >= 0)
            {
                return searchedRecord;
            }
            else
            {
                throw new ArgumentException($"There is no record with {nameof(id)} == {id}", nameof(id));
            }
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            var recordBuffer = new byte[RecordSize];

            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < this.fileStream.Length; i += RecordSize)
            {
                this.fileStream.Read(recordBuffer, 0, RecordSize);
                records.Add(BytesToFileCabinetRecord(recordBuffer));
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return (int)(this.fileStream.Length / RecordSize);
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

            var bytes = new byte[RecordSize];
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

        private static FileCabinetRecord BytesToFileCabinetRecord(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length < RecordSize)
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

        private static byte[] FileCabinetRecordToBytes(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            var bytes = new byte[RecordSize];
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
