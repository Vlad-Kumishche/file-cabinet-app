using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Class provides serialization of FileCabinetRecord to CSV.
    /// </summary>
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

        /// <summary>
        /// Writes first line of CSV file.
        /// </summary>
        public void WriteFirstLine()
        {
            this.writer.WriteLine("Id,First Name,Last Name,Date of Birth,Height,Cash Savings,Favorite Letter,");
        }

        /// <summary>
        /// Writes single record to the file.
        /// </summary>
        /// <param name="record">Record to write.</param>
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
