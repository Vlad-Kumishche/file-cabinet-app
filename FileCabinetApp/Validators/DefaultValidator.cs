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
            new FirstNameValidator(2, 60).ValidateParameters(recordToValidate);
            new LastNameValidator(2, 60).ValidateParameters(recordToValidate);
            new DateOfBirthValidator(new DateTime(1950, 1, 1), DateTime.Now).ValidateParameters(recordToValidate);
            new HeightValidator(40, 300).ValidateParameters(recordToValidate);
            new CashSavingsValidator(0M, 10_000_000M).ValidateParameters(recordToValidate);
            new LetterValidator().ValidateParameters(recordToValidate);
        }
    }
}
