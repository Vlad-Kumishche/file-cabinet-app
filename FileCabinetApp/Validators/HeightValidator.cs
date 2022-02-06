using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Height validator.
    /// </summary>
    public class HeightValidator : IRecordValidator
    {
        private short minHeight;
        private short maxHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeightValidator"/> class.
        /// </summary>
        /// <param name="minHeight">Min value.</param>
        /// <param name="maxHeight">Max value.</param>
        public HeightValidator(short minHeight, short maxHeight)
        {
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
        }

        /// <summary>
        /// Checks if the height is within a specified range.
        /// </summary>
        /// <param name="recordToValidate">Record to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the height does not match the specified range.</exception>
        public void ValidateParameters(RecordArgs recordToValidate)
        {
            short height = recordToValidate.Height;
            const short minHeight = 120;
            const short maxHeight = 250;
            if (height < minHeight || height > maxHeight)
            {
                throw new ArgumentException($"The {nameof(height)} is not within the allowed range.", nameof(recordToValidate));
            }
        }
    }
}
