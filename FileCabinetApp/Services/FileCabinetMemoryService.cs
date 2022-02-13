using System.Collections.ObjectModel;
using FileCabinetApp.Cache;
using FileCabinetApp.Collections;
using FileCabinetApp.Data;
using FileCabinetApp.Validators;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Сlass provides a service for storing file cabinet records in memory and operations on them.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private const string SelectAll = "*";
        private readonly List<FileCabinetRecord> list = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new (StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new (StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();
        private readonly IRecordValidator validator;
        private readonly Memoizator memoizator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">Specified validation strategy.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.validator = validator;
            this.memoizator = new Memoizator(this);
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordParameters recordToCreate)
        {
            this.validator.ValidateParameters(recordToCreate);
            int lastId = this.list.Count == 0 ? 0 : this.list[^1].Id;
            var record = new FileCabinetRecord(recordToCreate);
            if (record.Id == 0)
            {
                record.Id = lastId + 1;
            }

            this.list.Add(record);
            this.AddRecordToDictionaries(record);

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
                    this.memoizator.Clear();
                    return this.CreateRecord(recordToInsert);
                }

                throw new ArgumentException("A record with the given Id already exists.", nameof(recordToInsert));
            }

            this.memoizator.Clear();
            return this.CreateRecord(recordToInsert);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<int> Update(List<KeyValuePair<string, string>> newParameters, List<KeyValuePair<string, string>> searchOptions, string logicalOperator)
        {
            var recordsToUpdate = this.memoizator.SelectByOptions(searchOptions, logicalOperator);

            if (recordsToUpdate.Any())
            {
                var identifiersOfUpdatedRecords = new List<int>();
                foreach (var sourceRecord in recordsToUpdate)
                {
                    identifiersOfUpdatedRecords.Add(sourceRecord.Id);
                    var recordToUpdate = new RecordParameters(sourceRecord);
                    RecordParameters.UpdateRecordParams(ref recordToUpdate, newParameters);
                    this.EditRecord(recordToUpdate);
                }

                this.memoizator.Clear();
                return new (identifiersOfUpdatedRecords);
            }

            throw new ArgumentException("No records were found with the specified keys.");
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list);
        }

        /// <inheritdoc/>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            var loadedRecords = snapshot.Records;
            var importedRecordsCount = 0;

            foreach (var importedRecord in loadedRecords)
            {
                var recordParameters = new RecordParameters(importedRecord);

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
            var recordsToDelete = this.memoizator.SelectByOptions(searchOptions, logicalOperator);

            if (recordsToDelete.Any())
            {
                var identifiersOfRecordsToDelete = new List<int>();
                foreach (var record in recordsToDelete)
                {
                    identifiersOfRecordsToDelete.Add(record.Id);
                    this.Remove(record.Id);
                }

                this.memoizator.Clear();
                return new (identifiersOfRecordsToDelete);
            }

            throw new ArgumentException("No records were found.");
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> SelectByOptions(List<KeyValuePair<string, string>> searchOptions, string logicalOperator)
        {
            if (this.memoizator.TryGetRecords(searchOptions, logicalOperator, out var cachedRecords))
            {
                return cachedRecords;
            }

            if (IsNeedToSelectAll(searchOptions))
            {
                var records = this.GetRecords();
                this.memoizator.AddResult(searchOptions, logicalOperator, records);
                return records;
            }

            var listOfListsMatchesRecords = new List<IEnumerable<FileCabinetRecord>>();
            foreach (var searchOptionPair in searchOptions)
            {
                listOfListsMatchesRecords.Add(this.GetRecordsWith(searchOptionPair.Key, searchOptionPair.Value, logicalOperator));
            }

            IEnumerable<FileCabinetRecord>? selectedRecords = new List<FileCabinetRecord>();
            foreach (var listOfMatchesRecords in listOfListsMatchesRecords)
            {
                if (selectedRecords.Any())
                {
                    selectedRecords = logicalOperator switch
                    {
                        "and" => selectedRecords.Intersect(listOfMatchesRecords),
                        "or" => selectedRecords.Concat(listOfMatchesRecords),
                        _ => throw new ArgumentException($"Invalid logical operator '{logicalOperator}'")
                    };
                }
                else
                {
                    selectedRecords = listOfMatchesRecords;
                }
            }

            var resultOfSelection = new MemoryCollection(selectedRecords);
            this.memoizator.AddResult(searchOptions, logicalOperator, resultOfSelection);
            return resultOfSelection;
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecordById(int id)
        {
            var record = this.list.Find(x => x.Id == id);
            if (record == null)
            {
                throw new ArgumentException($"There is no record with {nameof(id)} == {id}", nameof(id));
            }

            return record;
        }

        /// <inheritdoc/>
        public (int, int) GetStat()
        {
            return (this.list.Count, 0);
        }

        /// <inheritdoc/>
        public void Purge()
        {
            Console.WriteLine("The memory service does not need to be defragmented.");
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

        private IEnumerable<FileCabinetRecord> GetRecordsWith(string key, string value, string logicalOperator)
        {
            bool keyIsValid = true;
            try
            {
                switch (key)
                {
                    case "id":
                        if (int.TryParse(value, out int id))
                        {
                            return new List<FileCabinetRecord>() { this.GetRecordById(id) };
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid {nameof(id)} value.");
                        }

                    case "firstname":
                        return this.FindByFirstName(value);

                    case "lastname":
                        return this.FindByLastName(value);

                    case "dateofbirth":
                        return this.FindByDateOfBirth(value);

                    case "height":
                        if (short.TryParse(value, out short height))
                        {
                            return this.list.FindAll(record => record.Height == height);
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid {nameof(height)} value.");
                        }

                    case "cashsavings":
                        if (decimal.TryParse(value, out decimal cashSavings))
                        {
                            return this.list.FindAll(record => record.CashSavings == cashSavings);
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid {nameof(cashSavings)} value.");
                        }

                    case "favoriteletter":
                        if (char.TryParse(value, out char favoriteLetter))
                        {
                            return this.list.FindAll(record => record.FavoriteLetter == favoriteLetter);
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid {nameof(favoriteLetter)} value.");
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
                    return new List<FileCabinetRecord>();
                }

                throw ex;
            }
        }

        private IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                var records = new ReadOnlyCollection<FileCabinetRecord>(this.firstNameDictionary[firstName]);
                return new MemoryCollection(records);
            }

            return new MemoryCollection();
        }

        private IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                var records = new ReadOnlyCollection<FileCabinetRecord>(this.lastNameDictionary[lastName]);
                return new MemoryCollection(records);
            }

            return new MemoryCollection();
        }

        private IEnumerable<FileCabinetRecord> FindByDateOfBirth(string sourceDate)
        {
            if (!DateTime.TryParse(sourceDate, out var dateOfBirth))
            {
                return new MemoryCollection();
            }

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                var records = new ReadOnlyCollection<FileCabinetRecord>(this.dateOfBirthDictionary[dateOfBirth]);
                return new MemoryCollection(records);
            }

            return new MemoryCollection();
        }

        private IEnumerable<FileCabinetRecord> GetRecords()
        {
            return new MemoryCollection(this.list);
        }

        private void EditRecord(RecordParameters recordToEdit)
        {
            this.validator.ValidateParameters(recordToEdit);
            var record = this.GetRecordById(recordToEdit.Id);
            this.RemoveRecordFromDictionaries(record);

            record.FirstName = recordToEdit.FirstName;
            record.LastName = recordToEdit.LastName;
            record.DateOfBirth = recordToEdit.DateOfBirth;
            record.Height = recordToEdit.Height;
            record.CashSavings = recordToEdit.CashSavings;
            record.FavoriteLetter = recordToEdit.FavoriteLetter;

            this.AddRecordToDictionaries(record);
        }

        private bool Remove(int recordId)
        {
            if (recordId < 1)
            {
                throw new ArgumentException($"The {nameof(recordId)} cannot be less than one.");
            }

            FileCabinetRecord recordToRemove;
            try
            {
                recordToRemove = this.GetRecordById(recordId);
            }
            catch (ArgumentException)
            {
                return false;
            }

            this.list.Remove(recordToRemove);
            this.RemoveRecordFromDictionaries(recordToRemove);
            return true;
        }

        private void RemoveRecordFromDictionaries(FileCabinetRecord recordToRemove)
        {
            recordToRemove.FirstName ??= string.Empty;
            recordToRemove.LastName ??= string.Empty;
            this.firstNameDictionary[recordToRemove.FirstName].Remove(recordToRemove);
            this.lastNameDictionary[recordToRemove.LastName].Remove(recordToRemove);
            this.dateOfBirthDictionary[recordToRemove.DateOfBirth].Remove(recordToRemove);
        }

        private void AddRecordToDictionaries(FileCabinetRecord record)
        {
            record.FirstName ??= string.Empty;
            record.LastName ??= string.Empty;

            if (this.firstNameDictionary.ContainsKey(record.FirstName))
            {
                this.firstNameDictionary[record.FirstName].Add(record);
            }
            else
            {
                this.firstNameDictionary[record.FirstName] = new () { record };
            }

            if (this.lastNameDictionary.ContainsKey(record.LastName))
            {
                this.lastNameDictionary[record.LastName].Add(record);
            }
            else
            {
                this.lastNameDictionary[record.LastName] = new () { record };
            }

            if (this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth))
            {
                this.dateOfBirthDictionary[record.DateOfBirth].Add(record);
            }
            else
            {
                this.dateOfBirthDictionary[record.DateOfBirth] = new () { record };
            }
        }
    }
}
