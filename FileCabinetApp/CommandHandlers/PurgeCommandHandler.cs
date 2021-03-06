using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for purge command.
    /// </summary>
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
            this.CommandName = "purge";
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            this.FileCabinetService.Purge();
            Console.WriteLine();
        }
    }
}
