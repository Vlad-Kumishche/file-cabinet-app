namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validator builder.
    /// </summary>
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators = new ();

        /// <summary>
        /// Creates the validator.
        /// </summary>
        /// <returns>The validator.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }

        /// <summary>
        /// Adds first name validator to builder.
        /// </summary>
        /// <param name="minLength">Min length.</param>
        /// <param name="maxLength">Max length.</param>
        /// <returns>The validator.</returns>
        public ValidatorBuilder ValidateFirstName(int minLength, int maxLength)
        {
            this.validators.Add(new FirstNameValidator(minLength, maxLength));
            return this;
        }

        /// <summary>
        /// Adds last name validator to builder.
        /// </summary>
        /// <param name="minLength">Min length.</param>
        /// <param name="maxLength">Max length.</param>
        /// <returns>The validator.</returns>
        public ValidatorBuilder ValidateLastName(int minLength, int maxLength)
        {
            this.validators.Add(new LastNameValidator(minLength, maxLength));
            return this;
        }

        /// <summary>
        /// Adds date of birth validator to builder.
        /// </summary>
        /// <param name="from">Min value.</param>
        /// <param name="to">Max value.</param>
        /// <returns>The validator.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        /// <summary>
        /// Adds height validator to builder.
        /// </summary>
        /// <param name="minHeight">Min value.</param>
        /// <param name="maxHeight">Max value.</param>
        /// <returns>The validator.</returns>
        public ValidatorBuilder ValidateHeight(short minHeight, short maxHeight)
        {
            this.validators.Add(new HeightValidator(minHeight, maxHeight));
            return this;
        }

        /// <summary>
        /// Adds cash savings validator to builder.
        /// </summary>
        /// <param name="minCashSavings">Min value.</param>
        /// <param name="maxCashSavings">Max value.</param>
        /// <returns>The validator.</returns>
        public ValidatorBuilder ValidateCashSavings(decimal minCashSavings, decimal maxCashSavings)
        {
            this.validators.Add(new CashSavingsValidator(minCashSavings, maxCashSavings));
            return this;
        }

        /// <summary>
        /// Adds letter validator to builder.
        /// </summary>
        /// <returns>The validator.</returns>
        public ValidatorBuilder ValidateLetter()
        {
            this.validators.Add(new LetterValidator());
            return this;
        }
    }
}
