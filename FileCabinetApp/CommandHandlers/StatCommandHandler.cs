using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for stat command.
    /// </summary>
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        public StatCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
            this.CommandName = "stat";
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            var recordsCount = this.FileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount.Item1} record(s) including {recordsCount.Item2} deleted records.");
            Console.WriteLine();
        }
    }
}
