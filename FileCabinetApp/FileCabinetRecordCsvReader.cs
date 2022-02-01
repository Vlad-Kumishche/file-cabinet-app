namespace FileCabinetApp
{
    /// <summary>
    /// Read records from csv file.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="streamReader">StreamReader.</param>
        public FileCabinetRecordCsvReader(StreamReader streamReader)
        {
            this.reader = streamReader;
        }

        /// <summary>
        /// Gets a list of records to import.
        /// </summary>
        /// <returns>List of records to import.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> readRecords = new List<FileCabinetRecord>();

            this.reader.ReadLine();
            while (!this.reader.EndOfStream)
            {
                var currentRecord = this.reader.ReadLine();
                currentRecord ??= string.Empty;
                var recordFields = currentRecord.Split(",");

                int id;
                DateTime dateOfBirth;
                short height;
                decimal cashSavings;
                char favoriteLetter;

                if (!int.TryParse(recordFields[0], out id))
                {
                    continue;
                }

                string dateToParse = recordFields[3][3..6] + recordFields[3][..3] + recordFields[3][6..];
                if (!DateTime.TryParse(dateToParse, out dateOfBirth))
                {
                    continue;
                }

                if (!short.TryParse(recordFields[4], out height))
                {
                    continue;
                }

                if (!decimal.TryParse(recordFields[5], out cashSavings))
                {
                    continue;
                }

                if (!char.TryParse(recordFields[6], out favoriteLetter))
                {
                    continue;
                }

                var record = new FileCabinetRecord
                {
                    Id = id,
                    FirstName = recordFields[1],
                    LastName = recordFields[2],
                    DateOfBirth = dateOfBirth,
                    Height = height,
                    CashSavings = cashSavings,
                    FavoriteLetter = favoriteLetter,
                };

                readRecords.Add(record);
            }

            return readRecords;
        }
    }
}
