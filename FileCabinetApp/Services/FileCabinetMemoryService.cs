using System.Collections.ObjectModel;
using System.Globalization;
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
        public int Insert(RecordArgs recordToInsert)
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
        public ReadOnlyCollection<int> Delete(string key, string value)
        {
            List<FileCabinetRecord> recordsToDelete = new ();
            List<int> identifiersOfRecordsToDelete = new ();

            switch (key)
            {
                case "id":
                    if (int.TryParse(value, out int id))
                    {
                        recordsToDelete = this.list.FindAll(record => record.Id == id);
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid {nameof(id)} value.");
                    }

                    break;

                case "firstname":
                    recordsToDelete = this.list.FindAll(record => record.FirstName == value);
                    break;

                case "lastname":
                    recordsToDelete = this.list.FindAll(record => record.LastName == value);
                    break;

                case "dateofbirth":
                    if (DateTime.TryParse(value, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dateOfBirth))
                    {
                        recordsToDelete = this.list.FindAll(record => record.DateOfBirth == dateOfBirth);
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid {nameof(dateOfBirth)} value.");
                    }

                    break;

                case "height":
                    if (short.TryParse(value, out short height))
                    {
                        recordsToDelete = this.list.FindAll(record => record.Height == height);
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid {nameof(height)} value.");
                    }

                    break;

                case "cashsavings":
                    if (decimal.TryParse(value, out decimal cashSavings))
                    {
                        recordsToDelete = this.list.FindAll(record => record.CashSavings == cashSavings);
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid {nameof(cashSavings)} value.");
                    }

                    break;

                case "favoriteletter":
                    if (char.TryParse(value, out char favoriteLetter))
                    {
                        recordsToDelete = this.list.FindAll(record => record.FavoriteLetter == favoriteLetter);
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid {nameof(favoriteLetter)} value.");
                    }

                    break;

                default:
                    throw new ArgumentException($"There is no {key} {nameof(key)}.");
            }

            if (recordsToDelete.Count > 0)
            {
                foreach (var record in recordsToDelete)
                {
                    identifiersOfRecordsToDelete.Add(record.Id);
                    this.Remove(record.Id);
                }

                return new (identifiersOfRecordsToDelete);
            }
            else
            {
                throw new ArgumentException("No records were found.");
            }
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                var records = new ReadOnlyCollection<FileCabinetRecord>(this.firstNameDictionary[firstName]);
                return new MemoryIterator(records);
            }

            return new MemoryIterator();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                var records = new ReadOnlyCollection<FileCabinetRecord>(this.lastNameDictionary[lastName]);
                return new MemoryIterator(records);
            }

            return new MemoryIterator();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string sourceDate)
        {
            if (!DateTime.TryParse(sourceDate, out var dateOfBirth))
            {
                return new MemoryIterator();
            }

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                var records = new ReadOnlyCollection<FileCabinetRecord>(this.dateOfBirthDictionary[dateOfBirth]);
                return new MemoryIterator(records);
            }

            return new MemoryIterator();
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
