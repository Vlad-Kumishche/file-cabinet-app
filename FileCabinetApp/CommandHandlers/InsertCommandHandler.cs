using System.Text.RegularExpressions;
using FileCabinetApp.Data;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for insert command.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        public InsertCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
            this.CommandName = "insert";
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            const int indexOfIdFieldInRecord = 0;
            const int attributeIndex = 1;
            const int attributeValueIndex = 2;
            const int maxRecordParamsCount = 7;

            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    throw new ArgumentNullException(nameof(parameters), $"The list of parameters for the '{this.CommandName}' command cannot be empty.");
                }

                var parameterRegex = new Regex(@"(.*) values (.*)", RegexOptions.IgnoreCase);
                if (parameterRegex.IsMatch(parameters))
                {
                    var matchParameters = parameterRegex.Match(parameters);
                    var attribute = matchParameters.Groups[attributeIndex].Value.ToLowerInvariant();
                    var attributeValue = matchParameters.Groups[attributeValueIndex].Value;
                    var recordFields = GetSubstrings(attribute);
                    var recordValues = GetSubstrings(attributeValue);

                    if (recordFields.Count != recordValues.Count || recordFields.Count > maxRecordParamsCount || (recordFields[indexOfIdFieldInRecord] != "id" && recordFields.Count != maxRecordParamsCount - 1))
                    {
                        throw new ArgumentException($"The max number of required parameters is {maxRecordParamsCount}.");
                    }

                    var parametersFromCommand = new List<KeyValuePair<string, string>>();
                    for (int i = 0; i < recordFields.Count; i++)
                    {
                        parametersFromCommand.Add(new (recordFields[i], recordValues[i]));
                    }

                    var recordForInsert = new RecordParameters();
                    RecordParameters.UpdateRecordParams(ref recordForInsert, parametersFromCommand);
                    var insertedRecordId = this.FileCabinetService.Insert(recordForInsert);
                    Console.WriteLine($"Record #{insertedRecordId} inserted.");
                }
                else
                {
                    throw new ArgumentException($"Invalid '{this.CommandName}' command.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"No record has been inserted. {ex.Message}");
            }

            Console.WriteLine();
        }
    }
}
