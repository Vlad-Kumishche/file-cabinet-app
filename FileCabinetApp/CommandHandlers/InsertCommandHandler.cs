using System.Globalization;
using System.Text.RegularExpressions;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Data;
using FileCabinetApp.Services;

namespace FileCabinetApp
{
    /// <summary>
    /// Handler for create command.
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
            const string invalidParamNamesOrOrder = "Invalid parameter names or order.";

            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    throw new ArgumentNullException(nameof(parameters), "The list of parameters for the 'insert' command cannot be empty.");
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

                    var recordForInsert = new RecordArgs();
                    for (int i = 0; i < recordFields.Count; i++)
                    {
                        switch (recordFields[i])
                        {
                            case "id":
                                if (int.TryParse(recordValues[i], out int id))
                                {
                                    recordForInsert.Id = id;
                                }
                                else
                                {
                                    throw new ArgumentException($"Invalid {nameof(id)} parameter.");
                                }

                                break;

                            case "firstname":
                                recordForInsert.FirstName = recordValues[i];
                                break;

                            case "lastname":
                                recordForInsert.LastName = recordValues[i];
                                break;

                            case "dateofbirth":
                                if (DateTime.TryParse(recordValues[i], new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dateOfBirth))
                                {
                                    recordForInsert.DateOfBirth = dateOfBirth;
                                }
                                else
                                {
                                    throw new ArgumentException($"Invalid {nameof(dateOfBirth)} parameter.");
                                }

                                break;

                            case "height":
                                if (short.TryParse(recordValues[i], out short height))
                                {
                                    recordForInsert.Height = height;
                                }
                                else
                                {
                                    throw new ArgumentException($"Invalid {nameof(height)} parameter.");
                                }

                                break;

                            case "cashsavings":
                                if (decimal.TryParse(recordValues[i], out decimal cashSavings))
                                {
                                    recordForInsert.CashSavings = cashSavings;
                                }
                                else
                                {
                                    throw new ArgumentException($"Invalid {nameof(cashSavings)} parameter.");
                                }

                                break;

                            case "favoriteletter":
                                if (char.TryParse(recordValues[i], out char favoriteLetter))
                                {
                                    recordForInsert.FavoriteLetter = favoriteLetter;
                                }
                                else
                                {
                                    throw new ArgumentException($"Invalid {nameof(favoriteLetter)} parameter.");
                                }

                                break;

                            default:
                                throw new ArgumentException(invalidParamNamesOrOrder);
                        }
                    }

                    var insertedRecordId = this.FileCabinetService.Insert(recordForInsert);
                    Console.WriteLine($"Record #{insertedRecordId} inserted.");
                    Console.WriteLine();
                }
                else
                {
                    throw new ArgumentException("Incorrect syntax for 'insert' command.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"No entry has been inserted. {ex.Message}");
            }

            static List<string> GetSubstrings(string inputString) => inputString.Split(new char[] { '(', ',', ')' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList<string>();
        }
    }
}
