namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for remove command.
    /// </summary>
    public class RemoveCommandHandler : CommandHandlerBase
    {
        private IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        public RemoveCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.CommandName = "remove";
            this.fileCabinetService = fileCabinetService;
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            if (parameters.Length == 0 || !int.TryParse(parameters, out var recordId))
            {
                Console.WriteLine("Invalid parameter. <param1> - record id.");
                return;
            }

            if (this.fileCabinetService.Remove(recordId))
            {
                Console.WriteLine($"Record #{recordId} is removed.");
                return;
            }

            Console.WriteLine($"Record #{recordId} doesn't exists.");
        }
    }
}
