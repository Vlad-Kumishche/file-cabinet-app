namespace FileCabinetApp.Validators
{
    public class ValidatorBuilder
    {
        private List<IRecordValidator> validators = new List<IRecordValidator>();

        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }

        public ValidatorBuilder ValidateFirstName(int minLength, int maxLength)
        {
            this.validators.Add(new FirstNameValidator(minLength, maxLength));
            return this;
        }

        public ValidatorBuilder ValidateLastName(int minLength, int maxLength)
        {
            this.validators.Add(new LastNameValidator(minLength, maxLength));
            return this;
        }

        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        public ValidatorBuilder ValidateHeight(short minHeight, short maxHeight)
        {
            this.validators.Add(new HeightValidator(minHeight, maxHeight));
            return this;
        }

        public ValidatorBuilder ValidateCashSavings(decimal minCashSavings, decimal maxCashSavings)
        {
            this.validators.Add(new CashSavingsValidator(minCashSavings, maxCashSavings));
            return this;
        }

        public ValidatorBuilder ValidateLetter()
        {
            this.validators.Add(new LetterValidator());
            return this;
        }
    }
}
