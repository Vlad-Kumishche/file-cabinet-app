using FileCabinetApp.Data;
using FileCabinetApp.Iterators;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for list command.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private readonly Action<IEnumerator<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        /// <param name="printer">Used printer.</param>
        public ListCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerator<FileCabinetRecord>> printer)
            : base(fileCabinetService)
        {
            this.CommandName = "list";
            this.printer = printer;
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            var records = this.FileCabinetService.GetRecords();
            var iterator = new MemoryIterator(records);
            this.printer(iterator);
        }
    }
}
