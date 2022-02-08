using FileCabinetApp.Data;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Record iterator interface.
    /// </summary>
    public interface IRecordIterator
    {
        /// <summary>
        /// Gets the next item.
        /// </summary>
        /// <returns>The next item.</returns>
        public FileCabinetRecord GetNext();

        /// <summary>
        /// Displays the presence of the next item.
        /// </summary>
        /// <returns>Returns true if there is another item.</returns>
        public bool HasMore();
    }
}