using System.Globalization;
using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    public class CustomDateOfBirthValidator : IRecordValidator
    {
        /// <summary>
        /// Checks if the date is within a specified range.
        /// </summary>
        /// <param name="recordToValidate">Record to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the length of the string does not match the specified range.</exception>
        public void ValidateParameters(RecordArgs recordToValidate)
        {
            DateTime dateOfBirth = recordToValidate.DateOfBirth;
            DateTime minDate = new DateTime(1940, 1, 1);
            const int ageOfMajority = 18;
            DateTime maxDate = DateTime.Now.AddYears(-ageOfMajority);
            if (dateOfBirth < minDate || dateOfBirth >= maxDate)
            {
                throw new ArgumentException($"Invalid {nameof(dateOfBirth)}. Min date: {minDate.ToString("MM / dd / yyyy", CultureInfo.InvariantCulture)}, max date: today.", nameof(recordToValidate));
            }
        }
    }
}
