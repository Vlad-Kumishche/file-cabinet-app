using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

        /// <summary>
        /// Writes snapshot of FileCabinetService to the CSV file.
        /// </summary>
        /// <param name="writer">Writer to file.</param>
        public void SaveToCsv(StreamWriter writer)
        {
            var csvWriter = new FileCabinetRecordCsvWriter(writer);
            csvWriter.WriteFirstLine();
            foreach (var record in this.records)
            {
                csvWriter.Write(record);
            }
        }

        /// <summary>
        /// Writes snapshot of FileCabinetService to the XML file.
        /// </summary>
        /// <param name="writer">Writer to file.</param>
        public void SaveToXml(XmlWriter writer)
        {
            var xmlWriter = new FileCabinetRecordXmlWriter(writer);
            xmlWriter.WriteFirstLine();
            foreach (var record in this.records)
            {
                xmlWriter.Write(record);
            }

            xmlWriter.WriteEndLine();
        }
    }
}
