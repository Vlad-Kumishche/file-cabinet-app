using System.Collections.ObjectModel;

namespace FileCabinetApp
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
        /// <returns>Number of records.</returns>
        int GetStat();
    }
}