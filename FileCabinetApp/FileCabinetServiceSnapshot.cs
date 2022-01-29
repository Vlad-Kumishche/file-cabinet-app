using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for snapshot of FileCabinetService.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">List of records.</param>
        public FileCabinetServiceSnapshot(List<FileCabinetRecord> records)
        {
            this.records = records.ToArray();
        }

        public void SaveToCsv(StreamWriter fileWriter)
        {
            var csvWriter = new FileCabinetRecordCsvWriter(fileWriter);
            csvWriter.WriteFirstLine();
            foreach (var record in this.records)
            {
                csvWriter.Write(record);
            }
        }
    }
}
