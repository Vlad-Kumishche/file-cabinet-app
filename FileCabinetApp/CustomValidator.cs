using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    internal class CustomValidator : IRecordValidator
    {
        /// <summary>
        /// Validates the file cabinet record.
        /// </summary>
        /// <param name="recordToValidate">Record to validate.</param>
        public void ValidateParameters(RecordArgs recordToValidate)
        {
            this.ValidateName(recordToValidate.FirstName);
            this.ValidateName(recordToValidate.LastName);
            this.ValidateDateOfBirth(recordToValidate.DateOfBirth);
            this.ValidateHeight(recordToValidate.Height);
            this.ValidateCashSavings(recordToValidate.CashSavings);
            this.ValidateLetter(recordToValidate.FavoriteLetter);
        }

        /// <summary>
        /// Сhecks if a string contains only English characters.
        /// </summary>
        /// <param name="line">String to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when string to validate is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when the length of the string does not match the given range.</exception>
        private void ValidateName(string? line)
        {
            const int minLength = 4;
            const int maxLength = 20;
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
                    this.ValidateLetter(c);
                }
            }
        }

        /// <summary>
        /// Checks if the date is within a specified range.
        /// </summary>
        /// <param name="dateOfBirth">Date to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the length of the string does not match the specified range.</exception>
        private void ValidateDateOfBirth(DateTime dateOfBirth)
        {
            DateTime minDate = new DateTime(1950, 1, 1);
            const int ageOfMajority = 18;
            DateTime maxDate = DateTime.Now.AddYears(-ageOfMajority);
            if (dateOfBirth < minDate || dateOfBirth >= maxDate)
            {
                throw new ArgumentException($"Invalid {nameof(dateOfBirth)}. Min date: {minDate.ToString("MM / dd / yyyy", CultureInfo.InvariantCulture)}, max date: {maxDate.ToString("MM / dd / yyyy", CultureInfo.InvariantCulture)}.", nameof(dateOfBirth));
            }
        }

        /// <summary>
        /// Checks if the height is within a specified range.
        /// </summary>
        /// <param name="height">Height to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the height does not match the specified range.</exception>
        private void ValidateHeight(short height)
        {
            const short minHeight = 120;
            const short maxHeight = 250;
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
        private void ValidateCashSavings(decimal cashSavings)
        {
            const decimal minCashSavings = 100M;
            const decimal maxCashSavings = 100_000_000M;
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
        private void ValidateLetter(char c)
        {
            if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z'))
            {
                throw new ArgumentException($"'{c}' is not an English letter.", nameof(c));
            }
        }
    }
}
