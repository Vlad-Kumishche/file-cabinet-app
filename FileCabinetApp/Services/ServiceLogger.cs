using System.Collections.ObjectModel;
using System.Globalization;
using FileCabinetApp.Data;

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
        public int CreateRecord(RecordParameters recordToCreate)
        {
            var recordId = this.service.CreateRecord(recordToCreate);

            Log($"Calling {nameof(this.service.CreateRecord)}() with " +
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
        public int Insert(RecordParameters recordToInsert)
        {
            var recordId = this.service.Insert(recordToInsert);
            Log($"Calling {nameof(this.service.Insert)}() with " +
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
        public ReadOnlyCollection<int> Update(List<KeyValuePair<string, string>> newParameters, List<KeyValuePair<string, string>> searchOptions, string logicalOperator)
        {
            var updatedRecordIds = this.service.Update(newParameters, searchOptions, logicalOperator);

            Log($"Calling {nameof(this.service.Update)}() with " +
                $"{nameof(newParameters)} = '{newParameters}'" +
                $"{nameof(searchOptions)} = '{searchOptions}'" +
                $"{nameof(logicalOperator)} = '{logicalOperator}'");
            Log($"{nameof(this.service.Update)}() returned '{updatedRecordIds}'");

            return updatedRecordIds;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> SelectByOptions(List<KeyValuePair<string, string>> searchOptions, string logicalOperator)
        {
            var selectedRecords = this.service.SelectByOptions(searchOptions, logicalOperator);

            Log($"Calling {nameof(this.service.SelectByOptions)}() with " +
                $"{nameof(searchOptions)} = '{searchOptions}'" +
                $"{nameof(logicalOperator)} = '{logicalOperator}'");
            Log($"{nameof(this.service.SelectByOptions)}() returned '{selectedRecords}'");

            return selectedRecords;
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecordById(int id)
        {
            var record = this.service.GetRecordById(id);

            Log($"Calling {nameof(this.service.GetRecordById)}() with " +
                $"{nameof(id)} = '{id}'");
            Log($"{nameof(this.service.GetRecordById)}() returned '{record}'");

            return record;
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
        public ReadOnlyCollection<int> Delete(List<KeyValuePair<string, string>> searchOptions, string logicalOperator)
        {
            var deletedRecordIds = this.service.Delete(searchOptions, logicalOperator);

            Log($"Calling {nameof(this.service.Delete)}() with " +
                $"{nameof(searchOptions)} = '{searchOptions}'" +
                $"{nameof(logicalOperator)} = '{logicalOperator}'");
            Log($"{nameof(this.service.Delete)}() returned '{deletedRecordIds}'");

            return deletedRecordIds;
        }

        /// <inheritdoc/>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            var amountOfRestoredRecords = this.service.Restore(snapshot);

            Log($"Calling {nameof(this.service.Restore)}() with " +
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