using System.Globalization;
using FileCabinetApp.Data;

namespace FileCabinetApp.Printers
{
    /// <summary>
    /// Default record printer.
    /// </summary>
    public class DefaultRecordPrinter : IRecordPrinter
    {
        /// <inheritdoc/>
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            foreach (var record in records)
            {
                string date = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {date}, {record.Height} cm, {record.CashSavings}$, {record.FavoriteLetter}");
            }
        }
    }
}