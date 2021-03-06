using System.Text.RegularExpressions;
using FileCabinetApp.Printers;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for select command.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private readonly IRecordPrinter printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        /// <param name="printer">Used printer.</param>
        public SelectCommandHandler(IFileCabinetService fileCabinetService, IRecordPrinter printer)
            : base(fileCabinetService)
        {
            this.CommandName = "select";
            this.printer = printer;
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            const int fieldsIndex = 1;
            const int searchOptionsIndex = 2;

            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    throw new ArgumentNullException(nameof(parameters), $"The list of parameters for the '{this.CommandName}' command cannot be empty.");
                }

                List<string> recordFieldsToSelect = new () { SelectAll };
                List<KeyValuePair<string, string>> recordSearchOptions = new () { new (SelectAll, SelectAll) };
                string logicalOperator = string.Empty;

                if (parameters.Trim() != SelectAll)
                {
                    var parametersRegex = new Regex(@"(.*) where (.*)", RegexOptions.IgnoreCase);
                    if (parametersRegex.IsMatch(parameters))
                    {
                        var matchParameters = parametersRegex.Match(parameters);
                        recordFieldsToSelect = GetSubstrings(matchParameters.Groups[fieldsIndex].Value.ToLowerInvariant());
                        recordSearchOptions = GetKeyValuePairsOfSearchOptions(matchParameters.Groups[searchOptionsIndex].Value, out logicalOperator);
                    }
                    else
                    {
                        throw new ArgumentException($"Incorrect syntax for {this.CommandName} command.");
                    }
                }

                var selectedRecords = this.FileCabinetService.SelectByOptions(recordSearchOptions, logicalOperator);
                this.printer.Print(selectedRecords, recordFieldsToSelect);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"The records have not been selected. {ex.Message}");
            }

            Console.WriteLine();
        }
    }
}
