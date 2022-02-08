using System.Collections.ObjectModel;
using FileCabinetApp.Data;
using FileCabinetApp.Iterators;
using FileCabinetApp.Validators;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Сlass provides a service for storing file cabinet records in filesystem and operations on them.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int RecordSize = 279;
        private const int MaxNameLength = 120;
        private const byte OffsetIsDelitedFlag = 2;
        private readonly Dictionary<string, List<long>> firstNameDictionary = new (StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<long>> lastNameDictionary = new (StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<DateTime, List<long>> dateOfBirthDictionary = new ();
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

            this.fileStream.Seek(0, SeekOrigin.End);
            var offset = this.fileStream.Position;

            this.AddRecordToDictionaries(record, offset);

            var bytesOfRecord = RecordToBytes(record);
            this.fileStream.Write(bytesOfRecord, 0, bytesOfRecord.Length);
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

            if (this.TryGetOffsetOfRecordWithId(record.Id, out var offset))
            {
                this.RemoveRecordFromDictionaries(this.GetRecordById(record.Id), offset);
                this.AddRecordToDictionaries(record, offset);

                var bytesOfRecord = RecordToBytes(record);
                this.fileStream.Seek(offset, SeekOrigin.Begin);
                this.fileStream.Write(bytesOfRecord, 0, bytesOfRecord.Length);
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

            if (this.TryGetOffsetOfRecordWithId(recordId, out var offset))
            {
                this.RemoveRecordFromDictionaries(this.GetRecordById(recordId), offset);

                byte firstPartOfStatus = 0;
                firstPartOfStatus ^= (byte)((-1 ^ firstPartOfStatus) & (1 << OffsetIsDelitedFlag));
                this.fileStream.Seek(offset, SeekOrigin.Begin);
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
                var recordAsBytes = RecordToBytes(record);
                this.fileStream.Write(recordAsBytes, 0, recordAsBytes.Length);
            }

            this.fileStream.Flush();
            this.fileStream.SetLength(this.fileStream.Position);
            this.deletedRecordsCount = 0;
            Console.WriteLine($"Data file processing is completed: {countOfRecordsBeforePurge - this.RecordsCount} of {countOfRecordsBeforePurge} records were purged.");
        }

        /// <inheritdoc/>
        public IRecordIterator FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                var offsets = this.firstNameDictionary[firstName];
                return new FilesystemIterator(this, offsets);
            }

            return new FilesystemIterator();
        }

        /// <inheritdoc/>
        public IRecordIterator FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                var offsets = this.lastNameDictionary[lastName];
                return new FilesystemIterator(this, offsets);
            }

            return new FilesystemIterator();
        }

        /// <inheritdoc/>
        public IRecordIterator FindByDateOfBirth(string sourceDate)
        {
            if (!DateTime.TryParse(sourceDate, out var dateOfBirth))
            {
                return new FilesystemIterator();
            }

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                var offsets = this.dateOfBirthDictionary[dateOfBirth];
                return new FilesystemIterator(this, offsets);
            }

            return new FilesystemIterator();
        }

        /// <summary>
        /// Gets id of last record.
        /// </summary>
        /// <returns>Id of last record.</returns>
        public int GetIdOfLastRecord()
        {
            var recordBuffer = new byte[RecordSize];
            if (this.RecordsCount > 0)
            {
                var offsetOfLastRecord = (this.RecordsCount - 1) * RecordSize;

                while (offsetOfLastRecord >= 0)
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

        /// <summary>
        /// Gets records by offset.
        /// </summary>
        /// <param name="offset">Offset.</param>
        /// <param name="record">Record.</param>
        /// <returns>Is successful.</returns>
        public bool TryGetRecordByOffset(long offset, out FileCabinetRecord record)
        {
            var recordBuffer = new byte[RecordSize];
            this.fileStream.Seek(offset, SeekOrigin.Begin);
            this.fileStream.Read(recordBuffer, 0, RecordSize);
            BytesToFileCabinetRecord(recordBuffer, out record, out var status);
            int isDeleted = (status >> OffsetIsDelitedFlag) & 1;
            if (isDeleted == 0)
            {
                return true;
            }

            return false;
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

        private bool TryGetOffsetOfRecordWithId(int id, out long offset)
        {
            offset = -1;
            this.fileStream.Seek(0, SeekOrigin.Begin);

            var recordBuffer = new byte[RecordSize];

            for (long currentOffset = 0; currentOffset < this.fileStream.Length; currentOffset += RecordSize)
            {
                this.fileStream.Read(recordBuffer, 0, RecordSize);
                FileCabinetRecord temporaryRecord;

                BytesToFileCabinetRecord(recordBuffer, out temporaryRecord, out var status);
                int isDeleted = (status >> OffsetIsDelitedFlag) & 1;
                if (temporaryRecord.Id == id)
                {
                    if (isDeleted == 0)
                    {
                        offset = currentOffset;
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

        private void RemoveRecordFromDictionaries(FileCabinetRecord recordToRemove, long offset)
        {
            recordToRemove.FirstName ??= string.Empty;
            recordToRemove.LastName ??= string.Empty;
            this.firstNameDictionary[recordToRemove.FirstName].Remove(offset);
            this.lastNameDictionary[recordToRemove.LastName].Remove(offset);
            this.dateOfBirthDictionary[recordToRemove.DateOfBirth].Remove(offset);
        }

        private void AddRecordToDictionaries(FileCabinetRecord record, long offset)
        {
            // to avoid CS8604
            record.FirstName ??= string.Empty;
            record.LastName ??= string.Empty;

            if (this.firstNameDictionary.ContainsKey(record.FirstName))
            {
                this.firstNameDictionary[record.FirstName].Add(offset);
            }
            else
            {
                this.firstNameDictionary[record.FirstName] = new () { offset };
            }

            if (this.lastNameDictionary.ContainsKey(record.LastName))
            {
                this.lastNameDictionary[record.LastName].Add(offset);
            }
            else
            {
                this.lastNameDictionary[record.LastName] = new () { offset };
            }

            if (this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth))
            {
                this.dateOfBirthDictionary[record.DateOfBirth].Add(offset);
            }
            else
            {
                this.dateOfBirthDictionary[record.DateOfBirth] = new () { offset };
            }
        }
    }
}
