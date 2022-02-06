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
            new FirstNameValidator(4, 20).ValidateParameters(recordToValidate);
            new LastNameValidator(4, 20).ValidateParameters(recordToValidate);
            new DateOfBirthValidator(new DateTime(1950, 1, 1), new DateTime(2005, 1, 1)).ValidateParameters(recordToValidate);
            new HeightValidator(120, 250).ValidateParameters(recordToValidate);
            new CashSavingsValidator(100M, 100_000_000M).ValidateParameters(recordToValidate);
            new LetterValidator().ValidateParameters(recordToValidate);
        }
    }
}
