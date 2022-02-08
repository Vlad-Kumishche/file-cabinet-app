using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Data;
using FileCabinetApp.Services;

namespace FileCabinetApp
{
    /// <summary>
    /// Handler for create command.
    /// </summary>
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        public CreateCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
            this.CommandName = "create";
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            var recordToCreate = new RecordArgs();

            Console.Write("First name: ");
            recordToCreate.FirstName = ReadInput(StringConverter, NameValidator);

            Console.Write("Last name: ");
            recordToCreate.LastName = ReadInput(StringConverter, NameValidator);

            Console.Write("Date of birth (MM/DD/YYYY): ");
            recordToCreate.DateOfBirth = ReadInput(DateConverter, DateValidator);

            Console.Write("Height (cm): ");
            recordToCreate.Height = ReadInput(ShortConverter, HeightValidator);

            Console.Write("Cash savings ($): ");
            recordToCreate.CashSavings = ReadInput(DecimalConverter, CashSavingsValidator);

            Console.Write("Favorite char: ");
            recordToCreate.FavoriteLetter = ReadInput(CharConverter, LetterValidator);

            int recordId = this.FileCabinetService.CreateRecord(recordToCreate);
            Console.WriteLine($"Record #{recordId} created.");
        }
    }
}
