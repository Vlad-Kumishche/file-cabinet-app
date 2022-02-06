using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class for default validation strategy.
    /// </summary>
    public class DefaultValidator : CompositeValidator
    {
        public DefaultValidator()
            : base(new List<IRecordValidator>
            {
                new FirstNameValidator(2, 60),
                new LastNameValidator(2, 60),
                new DateOfBirthValidator(new DateTime(1950, 1, 1), DateTime.Now),
                new HeightValidator(40, 300),
                new CashSavingsValidator(0M, 10_000_000M),
                new LetterValidator(),
            })
        {
        }
    }
}
