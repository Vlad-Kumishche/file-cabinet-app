using System.Collections.ObjectModel;
using FileCabinetApp.Data;
using FileCabinetApp.Iterators;
using FileCabinetApp.Validators;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Сlass provides a service for storing file cabinet records in memory and operations on them.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new (StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new (StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();
        private readonly IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">Specified validation strategy.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.validator = validator;
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordArgs recordToCreate)
        {
            this.validator.ValidateParameters(recordToCreate);
            int lastId = this.list.Count == 0 ? 0 : this.list[^1].Id;
            var record = new FileCabinetRecord
            {
                Id = (recordToCreate.Id == 0) ? lastId + 1 : recordToCreate.Id,
                FirstName = recordToCreate.FirstName,
                LastName = recordToCreate.LastName,
                DateOfBirth = recordToCreate.DateOfBirth,
                Height = recordToCreate.Height,
                CashSavings = recordToCreate.CashSavings,
                FavoriteLetter = recordToCreate.FavoriteLetter,
            };

            this.list.Add(record);
            this.AddRecordToDictionaries(record);

            return record.Id;
        }

        /// <inheritdoc/>
        public void EditRecord(RecordArgs recordToEdit)
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

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                var records = new ReadOnlyCollection<FileCabinetRecord>(this.firstNameDictionary[firstName]);
                return new RecordsCollection(records);
            }

            return new RecordsCollection();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                var records = new ReadOnlyCollection<FileCabinetRecord>(this.lastNameDictionary[lastName]);
                return new RecordsCollection(records);
            }

            return new RecordsCollection();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string sourceDate)
        {
            if (!DateTime.TryParse(sourceDate, out var dateOfBirth))
            {
                return new RecordsCollection();
            }

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                var records = new ReadOnlyCollection<FileCabinetRecord>(this.dateOfBirthDictionary[dateOfBirth]);
                return new RecordsCollection(records);
            }

            return new RecordsCollection();
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
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            var records = new ReadOnlyCollection<FileCabinetRecord>(this.list);
            return records;
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
