using System.Collections.ObjectModel;
using System.Globalization;
using FileCabinetApp.Cache;
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
        private const string SelectAll = "*";
        private readonly Dictionary<string, List<long>> firstNameDictionary = new (StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<long>> lastNameDictionary = new (StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<DateTime, List<long>> dateOfBirthDictionary = new ();
        private readonly Dictionary<short, List<long>> heightDictionary = new ();
        private readonly Dictionary<decimal, List<long>> cashSavingsDictionary = new ();
        private readonly Dictionary<char, List<long>> favoriteLetterDictionary = new ();
        private readonly IRecordValidator validator;

        /// <summary>
        /// File stream to file.
        /// </summary>
        private readonly FileStream fileStream;

        private int deletedRecordsCount;

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
        public int CreateRecord(RecordParameters recordToCreate)
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
        public int Insert(RecordParameters recordToInsert)
        {
            this.validator.ValidateParameters(recordToInsert);
            if (recordToInsert.Id != 0)
            {
                try
                {
                    this.GetRecordById(recordToInsert.Id);
                }
                catch
                {
                    return this.CreateRecord(recordToInsert);
                }

                throw new ArgumentException("A record with the given Id already exists.", nameof(recordToInsert));
            }

            return this.CreateRecord(recordToInsert);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<int> Update(List<KeyValuePair<string, string>> newParameters, List<KeyValuePair<string, string>> searchOptions, string logicalOperator)
        {
            var recordsToUpdate = this.SelectByOptions(searchOptions, logicalOperator);

            if (recordsToUpdate.Any())
            {
                var identifiersOfUpdatedRecords = new List<int>();
                foreach (var sourceRecord in recordsToUpdate)
                {
                    identifiersOfUpdatedRecords.Add(sourceRecord.Id);
                    var recordToUpdate = new RecordParameters()
                    {
                        Id = sourceRecord.Id,
                        FirstName = sourceRecord.FirstName,
                        LastName = sourceRecord.LastName,
                        DateOfBirth = sourceRecord.DateOfBirth,
                        Height = sourceRecord.Height,
                        CashSavings = sourceRecord.CashSavings,
                        FavoriteLetter = sourceRecord.FavoriteLetter,
                    };

                    UpdateRecordParams(recordToUpdate, newParameters);
                    this.EditRecord(recordToUpdate);
                }

                return new (identifiersOfUpdatedRecords);
            }
            else
            {
                throw new ArgumentException("No records were found with the specified keys.");
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
                var recordParameters = new RecordParameters()
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
                    this.validator.ValidateParameters(recordParameters);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error. Record #{importedRecord.Id} was not imported. Error message: {ex.Message}.");
                    continue;
                }

                importedRecordsCount++;
                try
                {
                    this.EditRecord(recordParameters);
                }
                catch
                {
                    this.CreateRecord(recordParameters);
                }
            }

            return importedRecordsCount;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<int> Delete(List<KeyValuePair<string, string>> searchOptions, string logicalOperator)
        {
            var recordsToDelete = this.SelectByOptions(searchOptions, logicalOperator);

            if (recordsToDelete.Any())
            {
                var identifiersOfRecordsToDelete = new List<int>();
                foreach (var record in recordsToDelete)
                {
                    identifiersOfRecordsToDelete.Add(record.Id);
                    this.RemoveById(record.Id);
                }

                return new (identifiersOfRecordsToDelete);
            }
            else
            {
                throw new ArgumentException("No records were found.");
            }
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
        public IEnumerable<FileCabinetRecord> SelectByOptions(List<KeyValuePair<string, string>> searchOptions, string logicalOperator)
        {
            if (IsNeedToSelectAll(searchOptions))
            {
                return this.GetRecords();
            }

            var listOfListsMatchesOffsets = new List<List<long>>();
            foreach (var searchOptionPair in searchOptions)
            {
                listOfListsMatchesOffsets.Add(this.GetOffsetsOfRecordsWith(searchOptionPair.Key, searchOptionPair.Value, logicalOperator));
            }

            IEnumerable<long> offsetsOfSelectedRecords = new List<long>();
            foreach (var listOfMatchesOffsets in listOfListsMatchesOffsets)
            {
                if (offsetsOfSelectedRecords.Any())
                {
                    offsetsOfSelectedRecords = logicalOperator switch
                    {
                        "and" => offsetsOfSelectedRecords.Intersect(listOfMatchesOffsets),
                        "or" => offsetsOfSelectedRecords.Concat(listOfMatchesOffsets),
                        _ => throw new ArgumentException($"Invalid logical operator '{logicalOperator}'")
                    };
                }
                else
                {
                    offsetsOfSelectedRecords = listOfMatchesOffsets;
                }
            }

            return new FilesystemIterator(this, offsetsOfSelectedRecords);
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
            FileCabinetRecord searchedRecord = new ();

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
            using var memoryStream = new MemoryStream(bytes);
            using var binaryReader = new BinaryReader(memoryStream);
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

        private static void UpdateRecordParams(RecordParameters recordToUpdate, List<KeyValuePair<string, string>> newParameters)
        {
            foreach (var newRecordParameter in newParameters)
            {
                switch (newRecordParameter.Key)
                {
                    case "id":
                        throw new ArgumentException("Update of the id field is prohibited.");

                    case "firstname":
                        recordToUpdate.FirstName = newRecordParameter.Value;

                        break;

                    case "lastname":
                        recordToUpdate.LastName = newRecordParameter.Value;
                        break;

                    case "dateofbirth":
                        if (DateTime.TryParse(newRecordParameter.Value, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dateOfBirth))
                        {
                            recordToUpdate.DateOfBirth = dateOfBirth;
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid '{nameof(dateOfBirth)}' value.");
                        }

                        break;

                    case "height":
                        if (short.TryParse(newRecordParameter.Value, out short height))
                        {
                            recordToUpdate.Height = height;
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid '{nameof(height)}' value.");
                        }

                        break;

                    case "cashsavings":
                        if (decimal.TryParse(newRecordParameter.Value, out decimal cashSavings))
                        {
                            recordToUpdate.CashSavings = cashSavings;
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid '{nameof(cashSavings)}' value.");
                        }

                        break;

                    case "favoriteletter":
                        if (char.TryParse(newRecordParameter.Value, out char favoriteLetter))
                        {
                            recordToUpdate.FavoriteLetter = favoriteLetter;
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid '{nameof(favoriteLetter)}' value.");
                        }

                        break;

                    default:
                        throw new ArgumentException($"Invalid key '{newRecordParameter.Key}'.");
                }
            }
        }

        private static bool IsNeedToSelectAll(List<KeyValuePair<string, string>> searchOptions)
        {
            var firstPair = searchOptions.GetEnumerator();
            firstPair.MoveNext();
            if (firstPair.Current.Key == SelectAll)
            {
                return true;
            }

            return false;
        }

        private void EditRecord(RecordParameters recordToEdit)
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

        private ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> records = new ();
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

        private bool RemoveById(int recordId)
        {
            if (recordId < 1)
            {
                throw new ArgumentException($"The {nameof(recordId)} cannot be less than one.");
            }

            if (this.TryGetOffsetOfRecordWithId(recordId, out var offset))
            {
                this.RemoveByOffset(offset);
                return true;
            }

            return false;
        }

        private bool RemoveByOffset(long offset)
        {
            if (offset < 0)
            {
                throw new ArgumentException($"The {nameof(offset)} cannot be less than zero.");
            }

            if (this.TryGetRecordByOffset(offset, out var record))
            {
                this.RemoveRecordFromDictionaries(record, offset);
                byte firstPartOfStatus = 0;
                firstPartOfStatus ^= (byte)((-1 ^ firstPartOfStatus) & (1 << OffsetIsDelitedFlag));
                this.fileStream.Seek(offset, SeekOrigin.Begin);
                this.fileStream.WriteByte(firstPartOfStatus);
                this.deletedRecordsCount++;
                return true;
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
            this.heightDictionary[recordToRemove.Height].Remove(offset);
            this.cashSavingsDictionary[recordToRemove.CashSavings].Remove(offset);
            this.favoriteLetterDictionary[recordToRemove.FavoriteLetter].Remove(offset);
        }

        private void AddRecordToDictionaries(FileCabinetRecord record, long offset)
        {
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

            if (this.heightDictionary.ContainsKey(record.Height))
            {
                this.heightDictionary[record.Height].Add(offset);
            }
            else
            {
                this.heightDictionary[record.Height] = new () { offset };
            }

            if (this.cashSavingsDictionary.ContainsKey(record.CashSavings))
            {
                this.cashSavingsDictionary[record.CashSavings].Add(offset);
            }
            else
            {
                this.cashSavingsDictionary[record.CashSavings] = new () { offset };
            }

            if (this.favoriteLetterDictionary.ContainsKey(record.FavoriteLetter))
            {
                this.favoriteLetterDictionary[record.FavoriteLetter].Add(offset);
            }
            else
            {
                this.favoriteLetterDictionary[record.FavoriteLetter] = new () { offset };
            }
        }

        private List<long> GetOffsetsOfRecordsWith(string key, string value, string logicalOperator)
        {
            bool keyIsValid = true;
            try
            {
                switch (key)
                {
                    case "id":
                        if (int.TryParse(value, out int id))
                        {
                            if (this.TryGetOffsetOfRecordWithId(id, out var offset))
                            {
                                return new () { offset };
                            }
                            else
                            {
                                throw new ArgumentException($"There is no records with {nameof(id)} = '{id}'.");
                            }
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid {nameof(id)} value.");
                        }

                    case "firstname":
                        string firstName = value;
                        if (this.TryGetOffsetsWhereFirstNameIs(firstName, out var firstNameOffsets))
                        {
                            return new (firstNameOffsets);
                        }
                        else
                        {
                            throw new ArgumentException($"There is no records with {nameof(firstName)} = '{firstName}'.");
                        }

                    case "lastname":
                        string lastName = value;
                        if (this.TryGetOffsetsWhereLastNameIs(lastName, out var lastNameOffsets))
                        {
                            return new (lastNameOffsets);
                        }
                        else
                        {
                            throw new ArgumentException($"There is no records with {nameof(lastName)} = '{lastName}'.");
                        }

                    case "dateofbirth":
                        string dateOfBirth = value;
                        if (this.TryGetOffsetsWhereDateOfBirthIs(dateOfBirth, out var dateOfBirthOffsets))
                        {
                            return new (dateOfBirthOffsets);
                        }
                        else
                        {
                            throw new ArgumentException($"There is no records with {nameof(dateOfBirth)} = '{dateOfBirth}'.");
                        }

                    case "height":
                        string height = value;
                        if (this.TryGetOffsetsWhereHeightIs(height, out var heightOffsets))
                        {
                            return new (heightOffsets);
                        }
                        else
                        {
                            throw new ArgumentException($"There is no records with {nameof(height)} = '{height}'.");
                        }

                    case "cashsavings":
                        string cashSavings = value;
                        if (this.TryGetOffsetsWhereCashSavingsIs(cashSavings, out var cashSavingsOffsets))
                        {
                            return new (cashSavingsOffsets);
                        }
                        else
                        {
                            throw new ArgumentException($"There is no records with {nameof(cashSavings)} = '{cashSavings}'.");
                        }

                    case "favoriteletter":
                        string favoriteLetter = value;
                        if (this.TryGetOffsetsWhereFavoriteLetterIs(favoriteLetter, out var favoriteLetterOffsets))
                        {
                            return new (favoriteLetterOffsets);
                        }
                        else
                        {
                            throw new ArgumentException($"There is no records with {nameof(favoriteLetter)} = '{favoriteLetter}'.");
                        }

                    default:
                        keyIsValid = false;
                        throw new ArgumentException($"There is no key like '{nameof(key)}'.");
                }
            }
            catch (ArgumentException ex)
            {
                if (logicalOperator == "or" && keyIsValid)
                {
                    return new ();
                }

                throw ex;
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

                BytesToFileCabinetRecord(recordBuffer, out FileCabinetRecord temporaryRecord, out var status);
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

        private bool TryGetOffsetsWhereFirstNameIs(string firstName, out List<long> offsets)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                offsets = this.firstNameDictionary[firstName];
                return true;
            }

            offsets = new List<long>();
            return false;
        }

        private bool TryGetOffsetsWhereLastNameIs(string lastName, out List<long> offsets)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                offsets = this.lastNameDictionary[lastName];
                return true;
            }

            offsets = new List<long>();
            return false;
        }

        private bool TryGetOffsetsWhereDateOfBirthIs(string sourceDate, out List<long> offsets)
        {
            offsets = new List<long>();
            if (!DateTime.TryParse(sourceDate, new CultureInfo("en-US"), DateTimeStyles.None, out var dateOfBirth))
            {
                return false;
            }

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                offsets = this.dateOfBirthDictionary[dateOfBirth];
                return true;
            }

            return false;
        }

        private bool TryGetOffsetsWhereHeightIs(string sourceHeight, out List<long> offsets)
        {
            offsets = new List<long>();
            if (!short.TryParse(sourceHeight, out var height))
            {
                return false;
            }

            if (this.heightDictionary.ContainsKey(height))
            {
                offsets = this.heightDictionary[height];
                return true;
            }

            return false;
        }

        private bool TryGetOffsetsWhereCashSavingsIs(string sourceCashSavings, out List<long> offsets)
        {
            offsets = new List<long>();
            if (!decimal.TryParse(sourceCashSavings, out var cashSavings))
            {
                return false;
            }

            if (this.cashSavingsDictionary.ContainsKey(cashSavings))
            {
                offsets = this.cashSavingsDictionary[cashSavings];
                return true;
            }

            return false;
        }

        private bool TryGetOffsetsWhereFavoriteLetterIs(string sourceLetter, out List<long> offsets)
        {
            offsets = new List<long>();
            if (!char.TryParse(sourceLetter, out var favoriteLetter))
            {
                return false;
            }

            if (this.favoriteLetterDictionary.ContainsKey(favoriteLetter))
            {
                offsets = this.favoriteLetterDictionary[favoriteLetter];
                return true;
            }

            return false;
        }
    }
}
