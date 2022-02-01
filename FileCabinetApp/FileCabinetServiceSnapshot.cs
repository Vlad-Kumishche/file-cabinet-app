using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for snapshot of FileCabinetService.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">List of records.</param>
        public FileCabinetServiceSnapshot(List<FileCabinetRecord> records)
        {
            this.records = records.ToArray();
        }

        /// <summary>
        /// Gets file cabinet records.
        /// </summary>
        /// <value>
        /// File cabinet records.
        /// </value>
        public ReadOnlyCollection<FileCabinetRecord> Records
        {
            get
            {
                return new ReadOnlyCollection<FileCabinetRecord>(this.records);
            }
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
        /// Loads records from file to snapshot.
        /// </summary>
        /// <param name="reader">Reader to file.</param>
        public void LoadFromCsv(StreamReader reader)
        {
            var csvReader = new FileCabinetRecordCsvReader(reader);
            var loadedRecords = csvReader.ReadAll();

            if (loadedRecords.Count == 0)
            {
                return;
            }

            this.records = loadedRecords.ToArray();
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

        /// <summary>
        /// Writes snapshot of FileCabinetService to the XML file via XmlSerializer.
        /// </summary>
        /// <param name="writer">Writer to file.</param>
        public void SaveToXmlWithXmlSerializer(XmlWriter writer)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<FileCabinetRecord>));

            ser.Serialize(writer, new List<FileCabinetRecord>(this.records));
        }
    }
}
