using FileCabinetApp.Data;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for edit command.
    /// </summary>
    public class EditCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        public EditCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
            this.CommandName = "edit";
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine($"#{parameters} record is not found.");
                return;
            }

            try
            {
                this.FileCabinetService.GetRecordById(id);
            }
            catch
            {
                Console.WriteLine($"#{parameters} record is not found.");
                return;
            }

            var recordToEdit = new RecordArgs();
            Console.Write("First name: ");
            recordToEdit.FirstName = ReadInput(StringConverter, NameValidator);

            Console.Write("Last name: ");
            recordToEdit.LastName = ReadInput(StringConverter, NameValidator);

            Console.Write("Date of birth (MM/DD/YYYY):");
            recordToEdit.DateOfBirth = ReadInput(DateConverter, DateValidator);

            Console.Write("Height (cm): ");
            recordToEdit.Height = ReadInput(ShortConverter, HeightValidator);

            Console.Write("Cash savings ($): ");
            recordToEdit.CashSavings = ReadInput(DecimalConverter, CashSavingsValidator);

            Console.Write("Favorite char: ");
            recordToEdit.FavoriteLetter = ReadInput(CharConverter, LetterValidator);

            recordToEdit.Id = id;
            this.FileCabinetService.EditRecord(recordToEdit);
            Console.WriteLine($"Record #{id} edited.");
        }
    }
}
