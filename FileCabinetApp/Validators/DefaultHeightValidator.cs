using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    public class DefaultHeightValidator : IRecordValidator
    {
        /// <summary>
        /// Checks if the height is within a specified range.
        /// </summary>
        /// <param name="recordToValidate">Record to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the height does not match the specified range.</exception>
        public void ValidateParameters(RecordArgs recordToValidate)
        {
            short height = recordToValidate.Height;
            const short minHeight = 40;
            const short maxHeight = 300;
            if (height < minHeight || height > maxHeight)
            {
                throw new ArgumentException($"The {nameof(height)} is not within the allowed range.", nameof(recordToValidate));
            }
        }
    }
}
