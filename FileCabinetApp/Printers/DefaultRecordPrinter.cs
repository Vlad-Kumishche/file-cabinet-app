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
        public void Print(IEnumerator<FileCabinetRecord> records)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            if (!records.MoveNext())
            {
                Console.WriteLine("Nothing found");
                return;
            }

            do
            {
                var record = records.Current;
                string date = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {date}, {record.Height} cm, {record.CashSavings}$, {record.FavoriteLetter}");
            }
            while (records.MoveNext());
        }
    }
}