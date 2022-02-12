using System.Text;
using System.Text.RegularExpressions;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for update command.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        public UpdateCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
            this.CommandName = "update";
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            const int newRecordParametersIndex = 1;
            const int searchOptionsIndex = 2;

            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    throw new ArgumentNullException(nameof(parameters), $"The list of parameters for the '{this.CommandName}' command cannot be empty.");
                }

                var parametersRegex = new Regex(@"set (.*) where (.*)", RegexOptions.IgnoreCase);
                if (parametersRegex.IsMatch(parameters))
                {
                    var matchParameters = parametersRegex.Match(parameters);
                    var newRecordParameters = GetKeyValuePairsOfParameters(matchParameters.Groups[newRecordParametersIndex].Value);
                    var recordSearchOptions = GetKeyValuePairsOfSearchOptions(matchParameters.Groups[searchOptionsIndex].Value, out var logicalOperator);

                    var identifiers = this.FileCabinetService.Update(newRecordParameters, recordSearchOptions, logicalOperator);
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

                    Console.WriteLine(" updated.");
                }
                else
                {
                    throw new ArgumentException($"Incorrect syntax for {this.CommandName} command.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"The records have not been updated. {ex.Message}");
            }

            Console.WriteLine();
        }

        private static List<KeyValuePair<string, string>> GetKeyValuePairsOfParameters(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var keyValuePairs = new List<KeyValuePair<string, string>>();
            var separatedParameters = parameters.Split(',');
            var parameterRegex = new Regex(@"(.*)=(.*)");

            foreach (var parameter in separatedParameters)
            {
                keyValuePairs.Add(GetPairOfParameters(parameterRegex, parameter));
            }

            return keyValuePairs;
        }
    }
}
