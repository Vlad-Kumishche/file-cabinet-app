using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Default last name validator.
    /// </summary>
    public class DefaultLastNameValidator : IRecordValidator
    {
        /// <summary>
        /// Сhecks if a string contains only English characters.
        /// </summary>
        /// <param name="recordToValidate">Record to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when string to validate is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when the length of the string does not match the given range.</exception>
        public void ValidateParameters(RecordArgs recordToValidate)
        {
            string? line = recordToValidate.FirstName;
            const int minLength = 2;
            const int maxLength = 60;
            if (string.IsNullOrEmpty(line))
            {
                throw new ArgumentNullException(nameof(recordToValidate));
            }
            else if (line.Length < minLength || line.Length > maxLength)
            {
                throw new ArgumentException($"{nameof(line)}.Length does not meet the requirements.", nameof(recordToValidate));
            }
            else
            {
                foreach (char c in line)
                {
                    ValidateLetter(c);
                }
            }
        }

        /// <summary>
        /// Checks if the character is an English letter.
        /// </summary>
        /// <param name="c">Character to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the character is not an English letter.</exception>
        private static void ValidateLetter(char c)
        {
            if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z'))
            {
                throw new ArgumentException($"'{c}' is not an English letter.", nameof(c));
            }
        }
    }
}
