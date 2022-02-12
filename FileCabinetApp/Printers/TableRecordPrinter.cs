using System.Globalization;
using System.Text;
using FileCabinetApp.Data;

namespace FileCabinetApp.Printers
{
    /// <summary>
    /// Prints records in a table.
    /// </summary>
    public class TableRecordPrinter : IRecordPrinter
    {
        private const char CrossingLines = '+';
        private const char HorizontalLine = '-';
        private const char VerticalLine = '|';
        private const string SelectAll = "*";

        private const string Id = "id";
        private const string FirstName = "firstname";
        private const string LastName = "lastname";
        private const string DateOfBirth = "dateofbirth";
        private const string Height = "height";
        private const string CashSavings = "cashsavings";
        private const string FavoriteLetter = "favoriteletter";

        private static readonly List<string> NamesOfRecordFields = new ()
        {
            nameof(Id),
            nameof(FirstName),
            nameof(LastName),
            nameof(DateOfBirth),
            nameof(Height),
            nameof(CashSavings),
            nameof(FavoriteLetter),
        };

        private static readonly Dictionary<string, int> MinimalFieldsLength = new ()
        {
            { Id, Id.Length },
            { FirstName, FirstName.Length },
            { LastName, LastName.Length },
            { DateOfBirth, DateOfBirth.Length },
            { Height, Height.Length },
            { CashSavings, CashSavings.Length },
            { FavoriteLetter, FavoriteLetter.Length },
        };

        /// <inheritdoc/>
        public void Print(IEnumerable<FileCabinetRecord> records, List<string> recordFieldsToPrint)
        {
            FillRecordFieldsIfNeedToPrintThemAll(ref recordFieldsToPrint);
            var fieldsLength = GetMaxFieldsLength(records);
            var horizontalTableLine = GetHorizontalDividingLine(recordFieldsToPrint, fieldsLength);

            Console.WriteLine(horizontalTableLine);
            Console.WriteLine(GetTableHead(recordFieldsToPrint, fieldsLength));
            Console.WriteLine(horizontalTableLine);

            foreach (var record in records)
            {
                Console.WriteLine(GetTableLine(record, recordFieldsToPrint, fieldsLength));
            }

            Console.WriteLine(horizontalTableLine);
        }

        private static void FillRecordFieldsIfNeedToPrintThemAll(ref List<string> recordFieldsToPrint)
        {
            var fieldsIterator = recordFieldsToPrint.GetEnumerator();
            fieldsIterator.MoveNext();
            var firstFieldToPrint = fieldsIterator.Current;
            if (firstFieldToPrint == SelectAll)
            {
                recordFieldsToPrint = new ()
                {
                    Id,
                    FirstName,
                    LastName,
                    DateOfBirth,
                    Height,
                    CashSavings,
                    FavoriteLetter,
                };
            }
        }

        private static string GetHorizontalDividingLine(List<string> recordFieldsToPrint, Dictionary<string, int> fieldsLength)
        {
            var horizontalTableLine = new StringBuilder();
            horizontalTableLine.Append(CrossingLines);
            foreach (var fieldToPrint in recordFieldsToPrint)
            {
                var lenghtOfCurrentField = GetFieldsLength(fieldToPrint, fieldsLength);
                horizontalTableLine.Insert(horizontalTableLine.Length, HorizontalLine.ToString(), lenghtOfCurrentField + 2);
                horizontalTableLine.Append(CrossingLines);
            }

            return horizontalTableLine.ToString();
        }

        private static string GetTableHead(List<string> recordFieldsToPrint, Dictionary<string, int> fieldsLength)
        {
            var tableHead = new StringBuilder();
            tableHead.Append(VerticalLine);
            foreach (var fieldToPrint in recordFieldsToPrint)
            {
                var lenghtOfCurrentField = GetFieldsLength(fieldToPrint, fieldsLength);
                tableHead.Append(FormattableString.Invariant($" {fieldToPrint.PadRight(lenghtOfCurrentField)} {VerticalLine}"));
            }

            return tableHead.ToString();
        }

        private static string GetTableLine(FileCabinetRecord record, List<string> recordFieldsToPrint, Dictionary<string, int> fieldsLength)
        {
            var tableLine = new StringBuilder();
            tableLine.Append(VerticalLine);
            foreach (var fieldToPrint in recordFieldsToPrint)
            {
                record.FirstName ??= string.Empty;
                record.LastName ??= string.Empty;

                var lenghtOfCurrentField = GetFieldsLength(fieldToPrint, fieldsLength);
                var tableCell = fieldToPrint switch
                {
                    Id => record.Id.ToString(CultureInfo.InvariantCulture).PadLeft(lenghtOfCurrentField),
                    FirstName => record.FirstName.PadRight(lenghtOfCurrentField),
                    LastName => record.LastName.PadRight(lenghtOfCurrentField),
                    DateOfBirth => record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture).PadLeft(lenghtOfCurrentField),
                    Height => record.Height.ToString(CultureInfo.InvariantCulture).PadLeft(lenghtOfCurrentField),
                    CashSavings => record.CashSavings.ToString(CultureInfo.InvariantCulture).PadLeft(lenghtOfCurrentField),
                    FavoriteLetter => record.FavoriteLetter.ToString().PadRight(lenghtOfCurrentField),
                    _ => throw new ArgumentException($"Invalid field name '{fieldToPrint}'"),
                };

                tableLine.Append(FormattableString.Invariant($" {tableCell} "));
                tableLine.Append(VerticalLine);
            }

            return tableLine.ToString();
        }

        private static Dictionary<string, int> GetMaxFieldsLength(IEnumerable<FileCabinetRecord> records)
        {
            if (!records.Any())
            {
                return MinimalFieldsLength;
            }

            var maxFieldsLength = new Dictionary<string, int>();
            foreach (var nameOfRecordField in NamesOfRecordFields)
            {
                var nameOfFieldToLower = nameOfRecordField.ToLowerInvariant();
                var maxLenght = nameOfFieldToLower switch
                {
                    Id => records.Max(record => record.Id.ToString(CultureInfo.InvariantCulture).Length),
                    FirstName => records.Max(record => (record.FirstName ?? string.Empty).Length),
                    LastName => records.Max(record => (record.LastName ?? string.Empty).Length),
                    DateOfBirth => records.Max(record => record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture).Length),
                    Height => records.Max(record => record.Height.ToString(CultureInfo.InvariantCulture).Length),
                    CashSavings => records.Max(record => record.CashSavings.ToString(CultureInfo.InvariantCulture).Length),
                    FavoriteLetter => MinimalFieldsLength[FavoriteLetter],
                    _ => throw new ArgumentException($"Invalid field name '{nameOfRecordField}'"),
                };

                maxLenght = Math.Max(MinimalFieldsLength[nameOfFieldToLower], maxLenght);
                maxFieldsLength.Add(nameOfFieldToLower, maxLenght);
            }

            return maxFieldsLength;
        }

        private static int GetFieldsLength(string fieldToPrint, Dictionary<string, int> fieldsLength)
        {
            try
            {
                return fieldsLength[fieldToPrint];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException($"There is no field like '{fieldToPrint}'");
            }
        }
    }
}
