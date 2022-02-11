using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Interface for validation strategy.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates the file cabinet record.
        /// </summary>
        /// <param name="recordToValidate">Record to validate.</param>
        public void ValidateParameters(RecordParameters recordToValidate);
    }
}
