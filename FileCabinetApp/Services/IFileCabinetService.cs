using System.Collections.ObjectModel;
using FileCabinetApp.Data;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Interface provides a service for storing file cabinet records and operations on them.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Creates file cabinet record.
        /// </summary>
        /// <param name="recordToCreate">Record to create.</param>
        /// <returns>The id of the record.</returns>
        int CreateRecord(RecordParameters recordToCreate);

        /// <summary>
        /// Inserts file cabinet record.
        /// </summary>
        /// <param name="recordToInsert">Record to insert.</param>
        /// <returns>The id of the record.</returns>
        public int Insert(RecordParameters recordToInsert);

        /// <summary>
        /// Updates a records with specified parameters.
        /// </summary>
        /// <param name="newParameters">A set of new parameters.</param>
        /// <param name="searchOptions">A set of parameters to select the records to update.</param>
        /// <param name="logicalOperator">Logical operator.</param>
        /// <returns>The list of updated records ids.</returns>
        public ReadOnlyCollection<int> Update(List<KeyValuePair<string, string>> newParameters, List<KeyValuePair<string, string>> searchOptions, string logicalOperator);

        /// <summary>
        /// Makes snapshot of current class state.
        /// </summary>
        /// <returns>Snapshot of FileCabinetService.</returns>
        FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Returns a selection of records based on search parameters.
        /// </summary>
        /// <param name="searchOptions">A set of parameters to select the records.</param>
        /// <param name="logicalOperator">Logical operator.</param>
        /// <returns>Selection of records.</returns>
        public IEnumerable<FileCabinetRecord> SelectByOptions(List<KeyValuePair<string, string>> searchOptions, string logicalOperator);

        /// <summary>
        /// Gets record by id.
        /// </summary>
        /// <param name="id">The id of the record.</param>
        /// <returns>Required file cabinet record.</returns>
        /// <exception cref="ArgumentException">Thrown when record with given id was not found.</exception>
        FileCabinetRecord GetRecordById(int id);

        /// <summary>
        /// Gets the number of records.
        /// </summary>
        /// <returns>Number of active and deleted records.</returns>
        (int, int) GetStat();

        /// <summary>
        /// Restores file cabinet records from snapshot.
        /// </summary>
        /// <param name="snapshot">snapshot.</param>
        /// <returns>Number of restores records.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Deletes the record with specified key and value.
        /// </summary>
        /// <param name="searchOptions">A set of parameters to select the records to delete.</param>
        /// <param name="logicalOperator">Logical operator.</param>
        /// <returns>The list of deleted record ids.</returns>
        public ReadOnlyCollection<int> Delete(List<KeyValuePair<string, string>> searchOptions, string logicalOperator);

        /// <summary>
        /// Defragments the data file.
        /// </summary>
        public void Purge();
    }
}