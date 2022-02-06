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
        public static IRecordValidator CreateDefault(this ValidatorBuilder validatorBuilder)
        {
            return validatorBuilder
                .ValidateFirstName(2, 60)
                .ValidateLastName(2, 60)
                .ValidateDateOfBirth(new DateTime(1950, 1, 1), DateTime.Today)
                .ValidateHeight(40, 300)
                .ValidateCashSavings(0M, 10_000_000M)
                .ValidateLetter()
                .Create();
        }

        /// <summary>
        /// Extension method for create custom record validator.
        /// </summary>
        /// <param name="validatorBuilder">Validator builder.</param>
        /// <returns>Custom record validator.</returns>
        public static IRecordValidator CreateCustom(this ValidatorBuilder validatorBuilder)
        {
            return validatorBuilder
                .ValidateFirstName(4, 20)
                .ValidateLastName(4, 20)
                .ValidateDateOfBirth(new DateTime(1990, 1, 1), DateTime.Today)
                .ValidateHeight(120, 250)
                .ValidateCashSavings(100M, 100_000_000M)
                .ValidateLetter()
                .Create();
        }
    }
}