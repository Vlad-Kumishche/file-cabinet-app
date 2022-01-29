using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Сlass provides a service for storing file cabinet records and operations on them.
    /// </summary>
    public class FileCabinetService : IFileCabinetService
    {
        private static readonly ReadOnlyCollection<FileCabinetRecord> EmptyRecordReadOnlyCollection = new List<FileCabinetRecord>().AsReadOnly();
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private readonly IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="validator">Specified validation strategy.</param>
        public FileCabinetService(IRecordValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Creates file cabinet record.
        /// </summary>
        /// <param name="recordToCreate">Record to create.</param>
        /// <returns>The id of the record.</returns>
        public int CreateRecord(RecordArgs recordToCreate)
        {
            this.validator.ValidateParameters(recordToCreate);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = recordToCreate.FirstName,
                LastName = recordToCreate.LastName,
                DateOfBirth = recordToCreate.DateOfBirth,
                Height = recordToCreate.Height,
                CashSavings = recordToCreate.CashSavings,
                FavoriteLetter = recordToCreate.FavoriteLetter,
            };

            this.list.Add(record);

            // to avoid CS8604
            record.FirstName ??= string.Empty;
            record.LastName ??= string.Empty;

            if (this.firstNameDictionary.ContainsKey(record.FirstName))
            {
                this.firstNameDictionary[record.FirstName].Add(record);
            }
            else
            {
                this.firstNameDictionary[record.FirstName] = new List<FileCabinetRecord>() { record };
            }

            if (this.lastNameDictionary.ContainsKey(record.LastName))
            {
                this.lastNameDictionary[record.LastName].Add(record);
            }
            else
            {
                this.lastNameDictionary[record.LastName] = new List<FileCabinetRecord>() { record };
            }

            if (this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth))
            {
                this.dateOfBirthDictionary[record.DateOfBirth].Add(record);
            }
            else
            {
                this.dateOfBirthDictionary[record.DateOfBirth] = new List<FileCabinetRecord>() { record };
            }

            return record.Id;
        }

        /// <summary>
        /// Edits specified file cabinet record.
        /// </summary>
        /// <param name="recordToEdit">Record to edit.</param>
        public void EditRecord(RecordArgs recordToEdit)
        {
            this.validator.ValidateParameters(recordToEdit);

            var record = this.GetRecordById(recordToEdit.Id);
            record.FirstName = recordToEdit.FirstName;
            record.LastName = recordToEdit.LastName;
            record.DateOfBirth = recordToEdit.DateOfBirth;
            record.Height = recordToEdit.Height;
            record.CashSavings = recordToEdit.CashSavings;
            record.FavoriteLetter = recordToEdit.FavoriteLetter;
        }

        /// <summary>
        /// Makes snapshot of current class state.
        /// </summary>
        /// <returns>Snapshot of FileCabinetService.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list);
        }

        /// <summary>
        /// Finds records by first name.
        /// </summary>
        /// <param name="firstName">The first name of the person.</param>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                var records = new ReadOnlyCollection<FileCabinetRecord>(this.firstNameDictionary[firstName]);
                return records;
            }

            return EmptyRecordReadOnlyCollection;
        }

        /// <summary>
        /// Finds records by last name.
        /// </summary>
        /// <param name="lastName">The last name of the person.</param>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                var records = new ReadOnlyCollection<FileCabinetRecord>(this.lastNameDictionary[lastName]);
                return records;
            }

            return EmptyRecordReadOnlyCollection;
        }

        /// <summary>
        /// Finds records by date of birth.
        /// </summary>
        /// <param name="sourceDate">The date of birth of the person.</param>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string sourceDate)
        {
            if (!DateTime.TryParse(sourceDate, out var dateOfBirth))
            {
                throw new ArgumentException($"Invalid {nameof(sourceDate)}", nameof(sourceDate));
            }

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                var records = new ReadOnlyCollection<FileCabinetRecord>(this.dateOfBirthDictionary[dateOfBirth]);
                return records;
            }

            return EmptyRecordReadOnlyCollection;
        }

        /// <summary>
        /// Gets record by id.
        /// </summary>
        /// <param name="id">The id of the record.</param>
        /// <returns>Required file cabinet record.</returns>
        /// <exception cref="ArgumentException">Thrown when record with given id was not found.</exception>
        public FileCabinetRecord GetRecordById(int id)
        {
            var record = this.list.Find(x => x.Id == id);
            if (record == null)
            {
                throw new ArgumentException($"There is no record with {nameof(id)} == {id}", nameof(id));
            }

            return record;
        }

        /// <summary>
        /// Gets all file cabinet records.
        /// </summary>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            var records = new ReadOnlyCollection<FileCabinetRecord>(this.list);
            return records;
        }

        /// <summary>
        /// Gets the number of records.
        /// </summary>
        /// <returns>Number of records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }
    }
}
