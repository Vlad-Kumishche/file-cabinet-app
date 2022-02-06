using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for commands using file cabinet service.
    /// </summary>
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        private IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        protected ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

        /// <summary>
        /// Gets used file cabinet service.
        /// </summary>
        /// <value>
        /// Used file cabinet service.
        /// </value>
        protected IFileCabinetService FileCabinetService => this.fileCabinetService;
    }
}
