using System.Text;
using System.Text.RegularExpressions;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Services;

namespace FileCabinetApp
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
            const string invalidCommandMessage = "Invalid 'delete' command.";

            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    throw new ArgumentNullException(nameof(parameters), "The list of parameters cannot be empty.");
                }

                var parametersRegex = new Regex(@"where (.*)=(.*)", RegexOptions.IgnoreCase);
                if (parametersRegex.IsMatch(parameters))
                {
                    var matchParameters = parametersRegex.Match(parameters);
                    var key = matchParameters.Groups[1].Value.ToLowerInvariant().Trim(' ');
                    var value = Regex.Match(matchParameters.Groups[2].Value, @"'(.*?)'").Groups[1].Value.Trim(' ');

                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    {
                        var identifiers = this.FileCabinetService.Delete(key, value);
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
                            Console.WriteLine(" is deleted.");
                        }
                        else
                        {
                            Console.WriteLine(" are deleted.");
                        }

                        Console.WriteLine();
                    }
                    else
                    {
                        throw new ArgumentException(invalidCommandMessage);
                    }
                }
                else
                {
                    throw new ArgumentException(invalidCommandMessage);
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"The records have not been deleted. {ex.Message}");
                Console.WriteLine();
            }
        }
    }
}
