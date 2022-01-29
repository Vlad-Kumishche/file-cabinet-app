using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="writer">Writer to file.</param>
        public FileCabinetRecordCsvWriter(StreamWriter writer)
        {
            this.writer = writer;
        }

        public void WriteFirstLine()
        {
            this.writer.WriteLine("Id,First Name,Last Name,Date of Birth,Height,Cash Savings,Favorite Letter,");
        }

        public void Write(FileCabinetRecord record)
        {
            var stringToWrite = new StringBuilder();
            const char delimiter = ',';
            stringToWrite.Append(record.Id.ToString(CultureInfo.InvariantCulture));
            stringToWrite.Append(delimiter);
            stringToWrite.Append(record.FirstName);
            stringToWrite.Append(delimiter);
            stringToWrite.Append(record.LastName);
            stringToWrite.Append(delimiter);
            stringToWrite.Append(record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
            stringToWrite.Append(delimiter);
            stringToWrite.Append(record.Height.ToString(CultureInfo.InvariantCulture));
            stringToWrite.Append(delimiter);
            stringToWrite.Append(record.CashSavings.ToString(CultureInfo.InvariantCulture));
            stringToWrite.Append(delimiter);
            stringToWrite.Append(record.FavoriteLetter);
            stringToWrite.Append(delimiter);

            this.writer.WriteLine(stringToWrite);
        }
    }
}
