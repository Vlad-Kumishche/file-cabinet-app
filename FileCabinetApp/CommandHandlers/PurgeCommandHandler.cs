namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for purge command.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase
    {
        private IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.CommandName = "purge";
            this.fileCabinetService = fileCabinetService;
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            if (this.fileCabinetService is FileCabinetFilesystemService)
            {
                this.fileCabinetService.Purge();
            }
        }
    }
}
