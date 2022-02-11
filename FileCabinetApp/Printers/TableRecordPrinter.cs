using FileCabinetApp.Data;

namespace FileCabinetApp.Printers
{
    /// <summary>
    /// Prints records in a table.
    /// </summary>
    public class TableRecordPrinter : IRecordPrinter
    {
        /// <inheritdoc/>
        public void Print(IEnumerator<FileCabinetRecord> records)
        {
            throw new NotImplementedException();
        }
    }
}
