using System.Collections.ObjectModel;
using FileCabinetApp.Data;

namespace FileCabinetApp.Service
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
        int CreateRecord(RecordArgs recordToCreate);

        /// <summary>
        /// Edits specified file cabinet record.
        /// </summary>
        /// <param name="recordToEdit">Record to edit.</param>
        void EditRecord(RecordArgs recordToEdit);

        /// <summary>
        /// Makes snapshot of current class state.
        /// </summary>
        /// <returns>Snapshot of FileCabinetService.</returns>
        FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Finds records by first name.
        /// </summary>
        /// <param name="firstName">The first name of the person.</param>
        /// <returns>Array of records.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Finds records by last name.
        /// </summary>
        /// <param name="lastName">The last name of the person.</param>
        /// <returns>Array of records.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Finds records by date of birth.
        /// </summary>
        /// <param name="sourceDate">The date of birth of the person.</param>
        /// <returns>Array of records.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string sourceDate);

        /// <summary>
        /// Gets record by id.
        /// </summary>
        /// <param name="id">The id of the record.</param>
        /// <returns>Required file cabinet record.</returns>
        /// <exception cref="ArgumentException">Thrown when record with given id was not found.</exception>
        FileCabinetRecord GetRecordById(int id);

        /// <summary>
        /// Gets all file cabinet records.
        /// </summary>
        /// <returns>Array of records.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

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
        /// Removes the record.
        /// </summary>
        /// <param name="recordId">Id of record for remove.</param>
        /// <returns>Whether the removal was successful.</returns>
        public bool Remove(int recordId);

        /// <summary>
        /// Defragments the data file.
        /// </summary>
        public void Purge();
    }
}