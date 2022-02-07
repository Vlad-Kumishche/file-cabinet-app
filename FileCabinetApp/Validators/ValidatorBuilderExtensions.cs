namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Provides extension methods for ValidatorBuilder.
    /// </summary>
    public static class ValidatorBuilderExtensions
    {
        /// <summary>
        /// Extension method for create default record validator.
        /// </summary>
        /// <param name="validatorBuilder">Validator builder.</param>
        /// <returns>Default record validator.</returns>
        public static IRecordValidator CreateDefault(this ValidatorBuilder validatorBuilder) => CreateValidator("default");

        /// <summary>
        /// Extension method for create custom record validator.
        /// </summary>
        /// <param name="validatorBuilder">Validator builder.</param>
        /// <returns>Custom record validator.</returns>
        public static IRecordValidator CreateCustom(this ValidatorBuilder validatorBuilder) => CreateValidator("custom");

        private static IRecordValidator CreateValidator(string validationType)
        {
            var configuration = new ValidationRulesConfigurationReader(validationType);
            (var minLenghtOfFirstName, var maxLengthOfFirstName) = configuration.ReadFirstNameValidationCriteria();
            (var minLenghtOfLastName, var maxLengthOfLastName) = configuration.ReadLastNameValidationCriteria();
            (var minDateOfBirth, var maxDateOfBirth) = configuration.ReadDateOfBirthValidationCriteria();
            (var minHeight, var maxHeight) = configuration.ReadHeightValidationCriteria();
            (var minCashSavings, var maxCashSavings) = configuration.ReadCashSavingsValidationCriteria();

            return new ValidatorBuilder()
                .ValidateFirstName(minLenghtOfFirstName, maxLengthOfFirstName)
                .ValidateLastName(minLenghtOfLastName, maxLengthOfLastName)
                .ValidateDateOfBirth(minDateOfBirth, maxDateOfBirth)
                .ValidateHeight(minHeight, maxHeight)
                .ValidateCashSavings(minCashSavings, maxCashSavings)
                .ValidateLetter()
                .Create();
        }
    }
}