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
        private const byte OffsetIsDelitedFlag = 2;
        private static readonly ReadOnlyCollection<FileCabinetRecord> EmptyRecordReadOnlyCollection = new List<FileCabinetRecord>().AsReadOnly();
        private readonly IRecordValidator validator;
        private int deletedRecordsCount;

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

        private int LastRecordId
        {
            get
            {
                return this.GetIdOfLastRecord();
            }
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordArgs recordToCreate)
        {
            this.validator.ValidateParameters(recordToCreate);
            var record = new FileCabinetRecord
            {
                Id = (recordToCreate.Id == 0) ? this.LastRecordId + 1 : recordToCreate.Id,
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

            var recordIndexToChange = -1;
            var recordBuffer = new byte[RecordSize];

            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int offset = 0, recordIndex = 0; offset < this.fileStream.Length; offset += RecordSize, recordIndex++)
            {
                this.fileStream.Read(recordBuffer, 0, RecordSize);
                BytesToFileCabinetRecord(recordBuffer, out var currendRecord, out var status);
                int isDeleted = (status >> OffsetIsDelitedFlag) & 1;
                if (currendRecord.Id == record.Id)
                {
                    if (isDeleted == 0)
                    {
                        recordIndexToChange = recordIndex;
                    }

                    break;
                }
            }

            if (recordIndexToChange >= 0)
            {
                var bytesOfRecordParameters = FileCabinetRecordToBytes(record);
                this.fileStream.Seek(recordIndexToChange * RecordSize, SeekOrigin.Begin);
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
            var records = new List<FileCabinetRecord>(this.GetRecords());
            return new FileCabinetServiceSnapshot(records);
        }

        /// <inheritdoc/>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            var loadedRecords = snapshot.Records;
            var importedRecordsCount = 0;
            foreach (var importedRecord in loadedRecords)
            {
                var recordArgs = new RecordArgs()
                {
                    Id = importedRecord.Id,
                    FirstName = importedRecord.FirstName,
                    LastName = importedRecord.LastName,
                    DateOfBirth = importedRecord.DateOfBirth,
                    Height = importedRecord.Height,
                    CashSavings = importedRecord.CashSavings,
                    FavoriteLetter = importedRecord.FavoriteLetter,
                };

                try
                {
                    this.validator.ValidateParameters(recordArgs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error. Record #{importedRecord.Id} was not imported. Error message: {ex.Message}.");
                    continue;
                }

                importedRecordsCount++;
                try
                {
                    this.EditRecord(recordArgs);
                }
                catch
                {
                    this.CreateRecord(recordArgs);
                }
            }

            return importedRecordsCount;
        }

        /// <inheritdoc/>
        public bool Remove(int recordId)
        {
            if (recordId < 1)
            {
                throw new ArgumentException($"The {nameof(recordId)} cannot be less than one.");
            }

            if (this.TryGetIndexOfRecordWithId(recordId, out var indexOfRecordForRemove))
            {
                this.fileStream.Seek(indexOfRecordForRemove * RecordSize, SeekOrigin.Begin);
                byte firstPartOfStatus = 0;
                firstPartOfStatus ^= (byte)((-1 ^ firstPartOfStatus) & (1 << OffsetIsDelitedFlag));
                this.fileStream.WriteByte(firstPartOfStatus);
                this.deletedRecordsCount++;
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void Purge()
        {
            var countOfRecordsBeforePurge = (int)(this.fileStream.Length / RecordSize);
            var records = this.GetRecords();

            this.fileStream.Seek(0, SeekOrigin.Begin);
            foreach (var record in records)
            {
                var recordAsBytes = FileCabinetRecordToBytes(record);
                this.fileStream.Write(recordAsBytes, 0, recordAsBytes.Length);
            }

            this.fileStream.Flush();
            this.fileStream.SetLength(this.fileStream.Position);
            this.deletedRecordsCount = 0;
            Console.WriteLine($"Data file processing is completed: {countOfRecordsBeforePurge - this.RecordsCount} of {countOfRecordsBeforePurge} records were purged.");
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> recordsFound = new List<FileCabinetRecord>();
            foreach (var record in this.GetRecords())
            {
                if (string.Equals(record.FirstName, firstName, StringComparison.OrdinalIgnoreCase))
                {
                    recordsFound.Add(record);
                }
            }

            var records = new ReadOnlyCollection<FileCabinetRecord>(recordsFound);
            return records;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<FileCabinetRecord> recordsFound = new List<FileCabinetRecord>();
            foreach (var record in this.GetRecords())
            {
                if (string.Equals(record.LastName, lastName, StringComparison.OrdinalIgnoreCase))
                {
                    recordsFound.Add(record);
                }
            }

            var records = new ReadOnlyCollection<FileCabinetRecord>(recordsFound);
            return records;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string sourceDate)
        {
            if (!DateTime.TryParse(sourceDate, out var dateOfBirth))
            {
                return EmptyRecordReadOnlyCollection;
            }

            List<FileCabinetRecord> recordsFound = new List<FileCabinetRecord>();
            foreach (var record in this.GetRecords())
            {
                if (record.DateOfBirth == dateOfBirth)
                {
                    recordsFound.Add(record);
                }
            }

            var records = new ReadOnlyCollection<FileCabinetRecord>(recordsFound);
            return records;
        }

        /// <summary>
        /// Gets id of last record.
        /// </summary>
        /// <returns>Id of last record.</returns>
        public int GetIdOfLastRecord()
        {
            var recordBuffer = new byte[RecordSize];
            if (this.RecordsCount > 1)
            {
                var offsetOfLastRecord = (this.RecordsCount - 1) * RecordSize;

                while (offsetOfLastRecord > 0)
                {
                    try
                    {
                        this.fileStream.Seek(offsetOfLastRecord, SeekOrigin.Begin);
                    }
                    catch (ArgumentException)
                    {
                        return 0;
                    }

                    this.fileStream.Read(recordBuffer, 0, RecordSize);
                    BytesToFileCabinetRecord(recordBuffer, out var record, out var status);
                    int isDeleted = (status >> OffsetIsDelitedFlag) & 1;
                    if (isDeleted == 0)
                    {
                        return record.Id;
                    }
                    else
                    {
                        offsetOfLastRecord -= RecordSize;
                    }
                }
            }

            return 0;
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecordById(int id)
        {
            var recordNumberToSearch = -1;
            var recordBuffer = new byte[RecordSize];
            FileCabinetRecord searchedRecord = new FileCabinetRecord();

            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 0, recordNumber = 0; i < this.fileStream.Length; i += RecordSize, recordNumber++)
            {
                this.fileStream.Read(recordBuffer, 0, RecordSize);
                BytesToFileCabinetRecord(recordBuffer, out searchedRecord, out var status);
                int isDeleted = (status >> OffsetIsDelitedFlag) & 1;
                if (searchedRecord.Id == id)
                {
                    if (isDeleted == 0)
                    {
                        recordNumberToSearch = recordNumber;
                    }

                    break;
                }
            }

            if (recordNumberToSearch >= 0)
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
                BytesToFileCabinetRecord(recordBuffer, out var record, out var status);
                int isDeleted = (status >> OffsetIsDelitedFlag) & 1;
                if (isDeleted == 0)
                {
                    records.Add(record);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public (int, int) GetStat()
        {
            int recordsCount = (int)(this.fileStream.Length / RecordSize);
            return (recordsCount, this.deletedRecordsCount);
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

        private static void BytesToFileCabinetRecord(byte[] bytes, out FileCabinetRecord record, out short status)
        {
            if (bytes.Length < RecordSize)
            {
                throw new ArgumentException($"Error. bytes.Lenth cannot be less than {RecordSize}.", nameof(bytes));
            }

            record = new FileCabinetRecord();
            using (var memoryStream = new MemoryStream(bytes))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                status = binaryReader.ReadInt16();
                if (status == 0)
                {
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
            }
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

        private bool TryGetIndexOfRecordWithId(int id, out int index)
        {
            index = -1;
            this.fileStream.Seek(0, SeekOrigin.Begin);

            var recordBuffer = new byte[RecordSize];

            for (int position = 0, i = 0; position < this.fileStream.Length; position += RecordSize, i++)
            {
                this.fileStream.Read(recordBuffer, 0, RecordSize);
                FileCabinetRecord temporaryRecord;

                BytesToFileCabinetRecord(recordBuffer, out temporaryRecord, out var status);
                int isDeleted = (status >> OffsetIsDelitedFlag) & 1;
                if (temporaryRecord.Id == id)
                {
                    if (isDeleted == 0)
                    {
                        index = i;
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }
    }
}
