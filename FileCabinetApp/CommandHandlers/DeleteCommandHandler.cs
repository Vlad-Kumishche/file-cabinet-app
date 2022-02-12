using System.Text;
using System.Text.RegularExpressions;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for delete command.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        public DeleteCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
            this.CommandName = "delete";
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            const int searchOptionsIndex = 1;
            string invalidCommandMessage = $"Invalid {this.CommandName} command.";

            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    throw new ArgumentNullException(nameof(parameters), "The list of parameters cannot be empty.");
                }

                List<KeyValuePair<string, string>> recordSearchOptions = new () { new (SelectAll, SelectAll) };
                string logicalOperator = string.Empty;

                if (parameters.Trim() != SelectAll)
                {
                    var parametersRegex = new Regex(@"where (.*)", RegexOptions.IgnoreCase);
                    if (parametersRegex.IsMatch(parameters))
                    {
                        var matchParameters = parametersRegex.Match(parameters);
                        recordSearchOptions = GetKeyValuePairsOfSearchOptions(matchParameters.Groups[searchOptionsIndex].Value, out logicalOperator);
                    }
                    else
                    {
                        throw new ArgumentException(invalidCommandMessage);
                    }
                }

                var identifiers = this.FileCabinetService.Delete(recordSearchOptions, logicalOperator);
                var message = new StringBuilder();
                for (int i = 0; i < identifiers.Count; i++)
                {
                    message.Append(FormattableString.Invariant($"#{identifiers[i]}"));
                    if (i < identifiers.Count - 1)
                    {
                        message.Append(", ");
                    }
                }

                Console.Write($"Record {message}");
                if (identifiers.Count == 1)
                {
                    Console.Write(" is");
                }
                else
                {
                    Console.Write(" are");
                }

                Console.WriteLine(" deleted.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"The records have not been deleted. {ex.Message}");
            }

            Console.WriteLine();
        }
    }
}
