namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for exit command.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private readonly Action<bool> changeIsRunningState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="changeIsRunningState">Delegate for closing the program.</param>
        public ExitCommandHandler(Action<bool> changeIsRunningState)
        {
            this.CommandName = "exit";
            this.changeIsRunningState = changeIsRunningState;
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            this.changeIsRunningState(false);
        }
    }
}
