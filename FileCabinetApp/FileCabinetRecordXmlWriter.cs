using System.Globalization;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Class provides serialization of FileCabinetRecord to XML.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly XmlWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">Writer to file.</param>
        public FileCabinetRecordXmlWriter(XmlWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes first line of XML file.
        /// </summary>
        public void WriteFirstLine()
        {
            this.writer.WriteStartDocument();
            this.writer.WriteStartElement("records");
        }

        /// <summary>
        /// Writes last line of XML file.
        /// </summary>
        public void WriteEndLine()
        {
            this.writer.WriteEndElement();
            this.writer.WriteEndDocument();
        }

        /// <summary>
        /// Writes single record to the file.
        /// </summary>
        /// <param name="record">Record to write.</param>
        public void Write(FileCabinetRecord record)
        {
            this.writer.WriteStartElement("record");
            this.writer.WriteAttributeString("id", record.Id.ToString(CultureInfo.InvariantCulture));

            this.writer.WriteStartElement("name");
            this.writer.WriteAttributeString("first", record.FirstName);
            this.writer.WriteAttributeString("last", record.LastName);
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("dateOfBirth");
            this.writer.WriteValue(record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("Height");
            this.writer.WriteValue(record.Height.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("CashSavings");
            this.writer.WriteValue(record.CashSavings.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("FavoriteLetter");
            this.writer.WriteValue(record.FavoriteLetter);
            this.writer.WriteEndElement();

            this.writer.WriteEndElement();
        }
    }
}
