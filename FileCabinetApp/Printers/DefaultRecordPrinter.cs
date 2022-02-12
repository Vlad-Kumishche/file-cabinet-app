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
        public void Print(IEnumerable<FileCabinetRecord> records, List<string> recordFieldsToPrint)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            var iterator = records.GetEnumerator();

            if (!iterator.MoveNext())
            {
                Console.WriteLine("Nothing found");
                return;
            }

            do
            {
                var record = iterator.Current;
                string date = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {date}, {record.Height} cm, {record.CashSavings}$, {record.FavoriteLetter}");
            }
            while (iterator.MoveNext());
        }
    }
}