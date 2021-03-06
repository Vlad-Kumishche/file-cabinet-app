using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Letter validator.
    /// </summary>
    public class LetterValidator : IRecordValidator
    {
        /// <summary>
        /// Checks if the character is an English letter.
        /// </summary>
        /// <param name="recordToValidate">Record to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the character is not an English letter.</exception>
        public void ValidateParameters(RecordParameters recordToValidate)
        {
            char c = recordToValidate.FavoriteLetter;
            if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z'))
            {
                throw new ArgumentException($"'{c}' is not an English letter.", nameof(recordToValidate));
            }
        }
    }
}
