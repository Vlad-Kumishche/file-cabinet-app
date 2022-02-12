using System.Globalization;

namespace FileCabinetApp.Data
{
    /// <summary>
    /// Class for introducing parameters for file cabinet record.
    /// </summary>
    public class RecordParameters
    {
        /// <summary>
        /// Gets or sets the id of the record.
        /// </summary>
        /// <value>The id of the record.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the first name of the person.
        /// </summary>
        /// <value>The first name of the person.</value>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the person.
        /// </summary>
        /// <value>The last name of the person.</value>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the person.
        /// </summary>
        /// <value>The date of birth of the person.</value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the height of the person.
        /// </summary>
        /// <value>The height of the person.</value>
        public short Height { get; set; }

        /// <summary>
        /// Gets or sets the cash savings of the person.
        /// </summary>
        /// <value>The cash savings of the person.</value>
        public decimal CashSavings { get; set; }

        /// <summary>
        /// Gets or sets the favorite letter of the person.
        /// </summary>
        /// <value>The favorite letter of the person.</value>
        public char FavoriteLetter { get; set; }
    }
}
