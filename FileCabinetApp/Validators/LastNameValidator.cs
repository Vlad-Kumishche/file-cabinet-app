using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Custom last name validator.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        private readonly int minLength;
        private readonly int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">Min length.</param>
        /// <param name="maxLength">Max length.</param>
        public LastNameValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        /// <summary>
        /// Сhecks if a string contains only English characters.
        /// </summary>
        /// <param name="recordToValidate">Record to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when string to validate is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when the length of the string does not match the given range.</exception>
        public void ValidateParameters(RecordParameters recordToValidate)
        {
            string? lastName = recordToValidate.LastName;
            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException(nameof(recordToValidate));
            }
            else if (lastName.Length < this.minLength || lastName.Length > this.maxLength)
            {
                throw new ArgumentException($"{nameof(lastName)}.Length does not meet the requirements.", nameof(recordToValidate));
            }
            else
            {
                new LetterValidator().ValidateParameters(recordToValidate);
            }
        }
    }
}
