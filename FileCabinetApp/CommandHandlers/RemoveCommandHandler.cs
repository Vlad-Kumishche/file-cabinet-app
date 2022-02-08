using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for remove command.
    /// </summary>
    public class RemoveCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        public RemoveCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
            this.CommandName = "remove";
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            if (parameters.Length == 0 || !int.TryParse(parameters, out var recordId))
            {
                Console.WriteLine("Invalid parameter. <param1> - record id.");
                return;
            }

            if (this.FileCabinetService.Remove(recordId))
            {
                Console.WriteLine($"Record #{recordId} is removed.");
                return;
            }

            Console.WriteLine($"Record #{recordId} doesn't exists.");
        }
    }
}
