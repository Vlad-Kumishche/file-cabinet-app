using System.Globalization;
using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Date of birth validator.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime from;
        private readonly DateTime to;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">Min value.</param>
        /// <param name="to">Max value.</param>
        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Checks if the date is within a specified range.
        /// </summary>
        /// <param name="recordToValidate">Record to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the length of the string does not match the specified range.</exception>
        public void ValidateParameters(RecordParameters recordToValidate)
        {
            DateTime dateOfBirth = recordToValidate.DateOfBirth;
            if (dateOfBirth < this.from || dateOfBirth >= this.to)
            {
                throw new ArgumentException($"Invalid {nameof(dateOfBirth)}. Min date: {this.from.ToString("MM / dd / yyyy", CultureInfo.InvariantCulture)}, max date: {this.to.ToString("MM / dd / yyyy", CultureInfo.InvariantCulture)}.", nameof(recordToValidate));
            }
        }
    }
}
