using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class for custom validation strategy.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public void ValidateParameters(RecordArgs recordToValidate)
        {
            new CustomFirstNameValidator().ValidateParameters(recordToValidate);
            new CustomLastNameValidator().ValidateParameters(recordToValidate);
            new CustomDateOfBirthValidator().ValidateParameters(recordToValidate);
            new CustomHeightValidator().ValidateParameters(recordToValidate);
            new CustomCashSavingsValidator().ValidateParameters(recordToValidate);
            new CustomLetterValidator().ValidateParameters(recordToValidate);
        }
    }
}
