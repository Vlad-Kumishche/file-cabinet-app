using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class for default validation strategy.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public void ValidateParameters(RecordArgs recordToValidate)
        {
            new DefaultFirstNameValidator().ValidateParameters(recordToValidate);
            new DefaultLastNameValidator().ValidateParameters(recordToValidate);
            new DefaultDateOfBirthValidator().ValidateParameters(recordToValidate);
            new DefaultHeightValidator().ValidateParameters(recordToValidate);
            new DefaultCashSavingsValidator().ValidateParameters(recordToValidate);
            new DefaultLetterValidator().ValidateParameters(recordToValidate);
        }
    }
}
