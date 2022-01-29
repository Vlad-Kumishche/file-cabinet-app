using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// Сlass provides a service for storing file cabinet records in filesystem and operations on them.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        /// <inheritdoc/>
        public int CreateRecord(RecordArgs recordToCreate)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void EditRecord(RecordArgs recordToEdit)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string sourceDate)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecordById(int id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            throw new NotImplementedException();
        }
    }
}
