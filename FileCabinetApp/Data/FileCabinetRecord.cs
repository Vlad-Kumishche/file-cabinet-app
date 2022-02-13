using System.Xml.Serialization;

namespace FileCabinetApp.Data
{
    /// <summary>
    /// Сlass represents the file cabinet record.
    /// </summary>
    [Serializable]
    public class FileCabinetRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecord"/> class.
        /// </summary>
        public FileCabinetRecord()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecord"/> class.
        /// </summary>
        /// <param name="record">Record to by copied.</param>
        public FileCabinetRecord(RecordParameters record)
        {
            this.Id = record.Id;
            this.FirstName = record.FirstName;
            this.LastName = record.LastName;
            this.DateOfBirth = record.DateOfBirth;
            this.Height = record.Height;
            this.CashSavings = record.CashSavings;
            this.FavoriteLetter = record.FavoriteLetter;
        }

        /// <summary>
        /// Gets or sets the id of the record.
        /// </summary>
        /// <value>The id of the record.</value>
        [XmlAttribute]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the first name of the person.
        /// </summary>
        /// <value>The first name of the person.</value>
        [XmlAttribute]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the person.
        /// </summary>
        /// <value>The last name of the person.</value>
        [XmlAttribute]
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the person.
        /// </summary>
        /// <value>The date of birth of the person.</value>
        [XmlAttribute]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the height of the person.
        /// </summary>
        /// <value>The height of the person.</value>
        [XmlAttribute]
        public short Height { get; set; }

        /// <summary>
        /// Gets or sets the cash savings of the person.
        /// </summary>
        /// <value>The cash savings of the person.</value>
        [XmlAttribute]
        public decimal CashSavings { get; set; }

        /// <summary>
        /// Gets or sets the favorite letter of the person.
        /// </summary>
        /// <value>The favorite letter of the person.</value>
        [XmlAttribute]
        public char FavoriteLetter { get; set; }
    }
}
