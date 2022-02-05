namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for list command.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        public ListCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
            this.CommandName = "list";
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            var records = this.fileCabinetService.GetRecords();
            ShowRecords(records);
        }
    }
}
