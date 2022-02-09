using System.Xml;
using System.Xml.Serialization;
using FileCabinetApp.Data;

namespace FileCabinetApp.Readers
{
    /// <summary>
    /// Loads information from xml file.
    /// </summary>
    internal class FileCabinetRecordXmlReader
    {
        private readonly XmlReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader">Xml reader.</param>
        public FileCabinetRecordXmlReader(XmlReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Reads records from csv file.
        /// </summary>
        /// <returns>List of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            IList<FileCabinetRecord> records;
            XmlSerializer serializer = new (typeof(List<FileCabinetRecord>));
            records = serializer.Deserialize(this.reader) as IList<FileCabinetRecord> ?? new List<FileCabinetRecord>();
            return records;
        }
    }
}