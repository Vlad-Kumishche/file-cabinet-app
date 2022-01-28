using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for default validation strategy.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <summary>
        /// Validates the file cabinet record.
        /// </summary>
        /// <param name="recordToValidate">Record to validate.</param>
        public void ValidateParameters(RecordArgs recordToValidate)
        {
            ValidateName(recordToValidate.FirstName);
            ValidateName(recordToValidate.LastName);
            ValidateDateOfBirth(recordToValidate.DateOfBirth);
            ValidateHeight(recordToValidate.Height);
            ValidateCashSavings(recordToValidate.CashSavings);
            ValidateLetter(recordToValidate.FavoriteLetter);
        }

        /// <summary>
        /// Сhecks if a string contains only English characters.
        /// </summary>
        /// <param name="line">String to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when string to validate is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when the length of the string does not match the given range.</exception>
        private static void ValidateName(string? line)
        {
            const int minLength = 2;
            const int maxLength = 60;
            if (string.IsNullOrEmpty(line))
            {
                throw new ArgumentNullException(nameof(line));
            }
            else if (line.Length < minLength || line.Length > maxLength)
            {
                throw new ArgumentException($"{nameof(line)}.Length does not meet the requirements.", nameof(line));
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
        /// Checks if the date is within a specified range.
        /// </summary>
        /// <param name="dateOfBirth">Date to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the length of the string does not match the specified range.</exception>
        private static void ValidateDateOfBirth(DateTime dateOfBirth)
        {
            DateTime minDate = new DateTime(1950, 1, 1);
            if (dateOfBirth < minDate || dateOfBirth >= DateTime.Now)
            {
                throw new ArgumentException($"Invalid {nameof(dateOfBirth)}. Min date: {minDate.ToString("MM / dd / yyyy", CultureInfo.InvariantCulture)}, max date: today.", nameof(dateOfBirth));
            }
        }

        /// <summary>
        /// Checks if the height is within a specified range.
        /// </summary>
        /// <param name="height">Height to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the height does not match the specified range.</exception>
        private static void ValidateHeight(short height)
        {
            const short minHeight = 40;
            const short maxHeight = 300;
            if (height < minHeight || height > maxHeight)
            {
                throw new ArgumentException($"The {nameof(height)} is not within the allowed range.", nameof(height));
            }
        }

        /// <summary>
        /// Checks if the cash savings is within a specified range.
        /// </summary>
        /// <param name="cashSavings">Cash savings to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the cash savings does not match the specified range.</exception>
        private static void ValidateCashSavings(decimal cashSavings)
        {
            const decimal minCashSavings = 0M;
            const decimal maxCashSavings = 10_000_000M;
            if (cashSavings < minCashSavings || cashSavings > maxCashSavings)
            {
                throw new ArgumentException($"The {nameof(cashSavings)} is not within the allowed range.", nameof(cashSavings));
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
