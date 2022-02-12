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

        /// <summary>
        /// Updates <paramref name="recordToUpdate"/> with <paramref name="newParameters"/>.
        /// </summary>
        /// <param name="recordToUpdate">Parameters to update.</param>
        /// <param name="newParameters">New Parameters.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="newParameters"/> contains non-existent key.</exception>
        public static void UpdateRecordParams(RecordParameters recordToUpdate, List<KeyValuePair<string, string>> newParameters)
        {
            foreach (var newRecordParameter in newParameters)
            {
                switch (newRecordParameter.Key)
                {
                    case "id":
                        throw new ArgumentException("Update of the id field is prohibited.");

                    case "firstname":
                        recordToUpdate.FirstName = newRecordParameter.Value;

                        break;

                    case "lastname":
                        recordToUpdate.LastName = newRecordParameter.Value;
                        break;

                    case "dateofbirth":
                        if (DateTime.TryParse(newRecordParameter.Value, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dateOfBirth))
                        {
                            recordToUpdate.DateOfBirth = dateOfBirth;
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid '{nameof(dateOfBirth)}' value.");
                        }

                        break;

                    case "height":
                        if (short.TryParse(newRecordParameter.Value, out short height))
                        {
                            recordToUpdate.Height = height;
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid '{nameof(height)}' value.");
                        }

                        break;

                    case "cashsavings":
                        if (decimal.TryParse(newRecordParameter.Value, out decimal cashSavings))
                        {
                            recordToUpdate.CashSavings = cashSavings;
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid '{nameof(cashSavings)}' value.");
                        }

                        break;

                    case "favoriteletter":
                        if (char.TryParse(newRecordParameter.Value, out char favoriteLetter))
                        {
                            recordToUpdate.FavoriteLetter = favoriteLetter;
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid '{nameof(favoriteLetter)}' value.");
                        }

                        break;

                    default:
                        throw new ArgumentException($"There is no key like '{newRecordParameter.Key}'.");
                }
            }
        }
    }
}
