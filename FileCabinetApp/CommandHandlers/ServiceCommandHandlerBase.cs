namespace FileCabinetApp.CommandHandlers
{
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        protected IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        protected ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }
    }
}
