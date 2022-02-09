using System.Collections.ObjectModel;
using System.Globalization;
using FileCabinetApp.Data;
using FileCabinetApp.Iterators;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// File cabinet service that logs information about service method calls and parameters.
    /// </summary>
    public class ServiceLogger : IFileCabinetService
    {
        private const string Path = "FileCabinetServiceLog.txt";
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        public ServiceLogger(IFileCabinetService fileCabinetService)
        {
            this.service = fileCabinetService;
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordArgs recordToCreate)
        {
            var recordId = this.service.CreateRecord(recordToCreate);

            Log($"Calling {nameof(this.service.CreateRecord)}() with" +
                $"{nameof(recordToCreate.FirstName)} = '{recordToCreate.FirstName}', " +
                $"{nameof(recordToCreate.LastName)} = '{recordToCreate.LastName}', " +
                $"{nameof(recordToCreate.DateOfBirth)} = '{recordToCreate.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}', " +
                $"{nameof(recordToCreate.Height)} = '{recordToCreate.Height}', " +
                $"{nameof(recordToCreate.CashSavings)} = '{recordToCreate.CashSavings}', " +
                $"{nameof(recordToCreate.FavoriteLetter)} = '{recordToCreate.FavoriteLetter}'");
            Log($"{nameof(this.service.CreateRecord)}() returned '{recordId}'");

            return recordId;
        }

        /// <inheritdoc/>
        public int Insert(RecordArgs recordToInsert)
        {
            var recordId = this.service.Insert(recordToInsert);
            Log($"Calling {nameof(this.service.Insert)}() with" +
                $"{nameof(recordToInsert.FirstName)} = '{recordToInsert.FirstName}', " +
                $"{nameof(recordToInsert.LastName)} = '{recordToInsert.LastName}', " +
                $"{nameof(recordToInsert.DateOfBirth)} = '{recordToInsert.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}', " +
                $"{nameof(recordToInsert.Height)} = '{recordToInsert.Height}', " +
                $"{nameof(recordToInsert.CashSavings)} = '{recordToInsert.CashSavings}', " +
                $"{nameof(recordToInsert.FavoriteLetter)} = '{recordToInsert.FavoriteLetter}'");
            Log($"{nameof(this.service.Insert)}() returned '{recordId}'");

            return recordId;
        }

        /// <inheritdoc/>
        public void EditRecord(RecordArgs recordToEdit)
        {
            this.service.EditRecord(recordToEdit);

            Log($"Calling {nameof(this.service.EditRecord)}() with" +
                $"{nameof(recordToEdit.FirstName)} = '{recordToEdit.FirstName}', " +
                $"{nameof(recordToEdit.LastName)} = '{recordToEdit.LastName}', " +
                $"{nameof(recordToEdit.DateOfBirth)} = '{recordToEdit.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}', " +
                $"{nameof(recordToEdit.Height)} = '{recordToEdit.Height}', " +
                $"{nameof(recordToEdit.CashSavings)} = '{recordToEdit.CashSavings}', " +
                $"{nameof(recordToEdit.FavoriteLetter)} = '{recordToEdit.FavoriteLetter}'");
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string sourceDate)
        {
            var records = this.service.FindByDateOfBirth(sourceDate);

            Log($"Calling {nameof(this.service.FindByDateOfBirth)}() with" +
                $"{nameof(sourceDate)} = '{sourceDate}'");
            Log($"{nameof(this.service.FindByDateOfBirth)}() returned '{records}'");

            return records;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            var records = this.service.FindByFirstName(firstName);

            Log($"Calling {nameof(this.service.FindByFirstName)}() with" +
                $"{nameof(firstName)} = '{firstName}'");
            Log($"{nameof(this.service.FindByFirstName)}() returned '{records}'");

            return records;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            var records = this.service.FindByLastName(lastName);

            Log($"Calling {nameof(this.service.FindByLastName)}() with" +
                $"{nameof(lastName)} = '{lastName}'");
            Log($"{nameof(this.service.FindByLastName)}() returned '{records}'");

            return records;
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecordById(int id)
        {
            var record = this.service.GetRecordById(id);

            Log($"Calling {nameof(this.service.GetRecordById)}() with" +
                $"{nameof(id)} = '{id}'");
            Log($"{nameof(this.service.GetRecordById)}() returned '{record}'");

            return record;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            var records = this.service.GetRecords();

            Log($"Calling {nameof(this.service.GetRecords)}()");
            Log($"{nameof(this.service.GetRecords)}() returned '{records}'");

            return records;
        }

        /// <inheritdoc/>
        public (int, int) GetStat()
        {
            var stat = this.service.GetStat();

            Log($"Calling {nameof(this.service.GetStat)}()");
            Log($"{nameof(this.service.GetStat)}() returned '{stat}'");

            return stat;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var snapshot = this.service.MakeSnapshot();

            Log($"Calling {nameof(this.service.MakeSnapshot)}()");
            Log($"{nameof(this.service.MakeSnapshot)}() returned '{snapshot}'");

            return snapshot;
        }

        /// <inheritdoc/>
        public void Purge()
        {
            this.service.Purge();

            Log($"Calling {nameof(this.service.Purge)}()");
        }

        /// <inheritdoc/>
        public bool Remove(int recordId)
        {
            var isSuccessful = this.service.Remove(recordId);

            Log($"Calling {nameof(this.service.Remove)}() with" +
                $"{nameof(recordId)} = '{recordId}'");
            Log($"{nameof(this.service.Remove)}() returned '{isSuccessful}'");

            return isSuccessful;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<int> Delete(string key, string value)
        {
            var deletedRecordIds = this.service.Delete(key, value);

            Log($"Calling {nameof(this.service.Delete)}() with" +
                $"{nameof(key)} = '{key}'" +
                $"{nameof(value)} = '{value}'");
            Log($"{nameof(this.service.Delete)}() returned '{deletedRecordIds}'");

            return deletedRecordIds;
        }

        /// <inheritdoc/>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            var amountOfRestoredRecords = this.service.Restore(snapshot);

            Log($"Calling {nameof(this.service.Restore)}() with" +
                $"{nameof(snapshot)} = '{snapshot}'");
            Log($"{nameof(this.service.Restore)}() returned '{amountOfRestoredRecords}'");

            return amountOfRestoredRecords;
        }

        private static void Log(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            using TextWriter textWriter = File.AppendText(Path);
            textWriter.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)} - {message}");
        }
    }
}