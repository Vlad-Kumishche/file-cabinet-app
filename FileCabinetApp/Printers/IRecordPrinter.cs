using FileCabinetApp.Data;

namespace FileCabinetApp.Printers
{
    /// <summary>
    /// Provides a printer for displaying records.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Prints the records.
        /// </summary>
        /// <param name="records">Records to print.</param>
        public void Print(IEnumerable<FileCabinetRecord> records);
    }
}
