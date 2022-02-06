using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class for custom validation strategy.
    /// </summary>
    public class CustomValidator : CompositeValidator
    {
        public CustomValidator()
            : base(new List<IRecordValidator>
            {
                new FirstNameValidator(4, 20),
                new LastNameValidator(4, 20),
                new DateOfBirthValidator(new DateTime(1950, 1, 1), new DateTime(2005, 1, 1)),
                new HeightValidator(120, 250),
                new CashSavingsValidator(100M, 100_000_000M),
                new LetterValidator(),
            })
        {
        }
    }
}
