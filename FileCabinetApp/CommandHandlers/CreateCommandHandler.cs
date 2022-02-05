using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for create command.
    /// </summary>
    public class CreateCommandHandler : CommandHandlerBase
    {
        private IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        public CreateCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.CommandName = "create";
            this.fileCabinetService = fileCabinetService;
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            var recordToCreate = new RecordArgs();

            Console.Write("First name: ");
            recordToCreate.FirstName = ReadInput(StringConverter, NameValidator);

            Console.Write("Last name: ");
            recordToCreate.LastName = ReadInput(StringConverter, NameValidator);

            Console.Write("Date of birth: ");
            recordToCreate.DateOfBirth = ReadInput(DateConverter, DateValidator);

            Console.Write("Height (cm): ");
            recordToCreate.Height = ReadInput(ShortConverter, HeightValidator);

            Console.Write("Cash savings ($): ");
            recordToCreate.CashSavings = ReadInput(DecimalConverter, CashSavingsValidator);

            Console.Write("Favorite char: ");
            recordToCreate.FavoriteLetter = ReadInput(CharConverter, LetterValidator);

            int recordId = this.fileCabinetService.CreateRecord(recordToCreate);
            Console.WriteLine($"Record #{recordId} created.");
        }
    }
}
