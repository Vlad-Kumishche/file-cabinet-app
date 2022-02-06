using System.Globalization;
using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    public class DateOfBirthValidator : IRecordValidator
    {
        private DateTime from;
        private DateTime to;

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
        public void ValidateParameters(RecordArgs recordToValidate)
        {
            DateTime dateOfBirth = recordToValidate.DateOfBirth;
            if (dateOfBirth < this.from || dateOfBirth >= this.to)
            {
                throw new ArgumentException($"Invalid {nameof(dateOfBirth)}. Min date: {this.from.ToString("MM / dd / yyyy", CultureInfo.InvariantCulture)}, max date: {this.to.ToString("MM / dd / yyyy", CultureInfo.InvariantCulture)}.", nameof(recordToValidate));
            }
        }
    }
}
