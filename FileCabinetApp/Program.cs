using System.Globalization;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Data;
using FileCabinetApp.Services;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Class provides user interaction.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Uladzislau Kumishcha";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string FileName = "cabinet-records.db";
        private const string DefaultValidationRulesValue = "default";
        private const string CustomValidationRulesValue = "custom";
        private static readonly Dictionary<string, SetRule> ParamsList = new ()
        {
            ["--storage"] = new SetRule(SetStorageRules),
            ["-s"] = new SetRule(SetStorageRules),
            ["--validation-rules"] = new SetRule(SetValidationRules),
            ["-v"] = new SetRule(SetValidationRules),
            ["--use"] = new SetRule(SetUseRules),
            ["-u"] = new SetRule(SetUseRules),
        };

        private static string currentValidationRules = "default";
        private static string currentStorageRules = "memory";
        private static IRecordValidator validator = new ValidatorBuilder().CreateDefault();
        private static bool isRunning = true;
        private static bool isServiceMeterEnable;
        private static bool isServiceLoggerEnable;
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(validator);

        private delegate void SetRule(string args);

        /// <summary>
        /// Gets name of current validation rules.
        /// </summary>
        /// <value>
        /// Name of current validation rules.
        /// </value>
        public static string CurrentValidationRules => currentValidationRules;

        /// <summary>
        /// Gets name of default validation rules.
        /// </summary>
        /// <value>
        /// Name of default validation rules.
        /// </value>
        public static string DefaultValidationRules => DefaultValidationRulesValue;

        /// <summary>
        /// Gets name of custom validation rules.
        /// </summary>
        /// <value>
        /// Name of custom validation rules.
        /// </value>
        public static string CustomValidationRules => CustomValidationRulesValue;

        /// <summary>
        /// Processes user input and calls the appropriate functions.
        /// </summary>
        /// <param name="args">Сommand line arguments.</param>
        public static void Main(string[] args)
        {
            ParseCommandLineParams(args);
            if (isServiceMeterEnable)
            {
                fileCabinetService = new ServiceMeter(fileCabinetService);
            }

            if (isServiceLoggerEnable)
            {
                fileCabinetService = new ServiceLogger(fileCabinetService);
            }

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine($"Using {CurrentValidationRules} validation rules.");
            Console.WriteLine($"Using {currentStorageRules} to store records");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            var commandHandler = CreateCommandHandlers();
            do
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                var inputs = line != null ? line.Split(' ', 2) : new string[] { string.Empty, string.Empty };
                const int commandIndex = 0;
                const int parametersIndex = 1;
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                commandHandler.Handle(new AppCommandRequest(command, parameters));
            }
            while (isRunning);
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpCommandHandler = new HelpCommandHandler();
            var exitCommandHandler = new ExitCommandHandler(x => isRunning = x);
            var statCommandHandler = new StatCommandHandler(fileCabinetService);
            var createCommandHandler = new CreateCommandHandler(fileCabinetService);
            var insertCommandHandler = new InsertCommandHandler(fileCabinetService);
            var listCommandHandler = new ListCommandHandler(fileCabinetService, DefaultRecordPrint);
            var editCommandHandler = new EditCommandHandler(fileCabinetService);
            var findCommandHandler = new FindCommandHandler(fileCabinetService, DefaultRecordPrint);
            var exportCommandHandler = new ExportCommandHandler(fileCabinetService);
            var importCommandHandler = new ImportCommandHandler(fileCabinetService);
            var removeCommandHandler = new RemoveCommandHandler(fileCabinetService);
            var deleteCommandHandler = new DeleteCommandHandler(fileCabinetService);
            var purgeCommandHandler = new PurgeCommandHandler(fileCabinetService);

            helpCommandHandler.SetNext(exitCommandHandler);
            exitCommandHandler.SetNext(statCommandHandler);
            statCommandHandler.SetNext(createCommandHandler);
            createCommandHandler.SetNext(insertCommandHandler);
            insertCommandHandler.SetNext(listCommandHandler);
            listCommandHandler.SetNext(editCommandHandler);
            editCommandHandler.SetNext(findCommandHandler);
            findCommandHandler.SetNext(exportCommandHandler);
            exportCommandHandler.SetNext(importCommandHandler);
            importCommandHandler.SetNext(removeCommandHandler);
            removeCommandHandler.SetNext(deleteCommandHandler);
            deleteCommandHandler.SetNext(purgeCommandHandler);

            return helpCommandHandler;
        }

        private static void ParseCommandLineParams(string[] args)
        {
            string[] currentArgs = new string[2];
            if (args != null && args.Length > 0)
            {
                for (int i = 1; i < args.Length; i++)
                {
                    currentArgs[0] = args[i - 1];
                    currentArgs[1] = args[i];
                    ParsePairOfParams(currentArgs);
                }
            }
        }

        private static void ParsePairOfParams(string[] args)
        {
            string operation = string.Empty;
            string parameter = string.Empty;
            SetRule? changeRule = null;
            const int first = 0;
            const int second = 1;

            if (args[first].StartsWith("--", StringComparison.InvariantCulture))
            {
                int index = args[first].IndexOf("=", StringComparison.InvariantCulture);
                if (index != -1)
                {
                    operation = args[first][..index];
                    parameter = args[first][(index + 1) ..];
                    if (ParamsList.ContainsKey(operation.ToLower(CultureInfo.InvariantCulture)))
                    {
                        changeRule = ParamsList[operation.ToLower(CultureInfo.InvariantCulture)];
                    }
                }
            }
            else if (args[first].StartsWith("-", StringComparison.InvariantCulture))
            {
                operation = args[first];
                if (args[second] != null)
                {
                    parameter = args[second];
                }

                if (ParamsList.ContainsKey(operation.ToLower(CultureInfo.InvariantCulture)))
                {
                    changeRule = ParamsList[operation.ToLower(CultureInfo.InvariantCulture)];
                }
            }
            else if (args[second].StartsWith("--", StringComparison.InvariantCulture))
            {
                int index = args[second].IndexOf("=", StringComparison.InvariantCulture);
                if (index != -1)
                {
                    operation = args[second][..index];
                    parameter = args[second][(index + 1) ..];
                    if (ParamsList.ContainsKey(operation.ToLower(CultureInfo.InvariantCulture)))
                    {
                        changeRule = ParamsList[operation.ToLower(CultureInfo.InvariantCulture)];
                    }
                }
            }

            if (changeRule != null && !string.IsNullOrEmpty(operation) && !string.IsNullOrEmpty(parameter))
            {
                changeRule.Invoke(parameter.ToLower(CultureInfo.InvariantCulture));
            }
        }

        private static void SetValidationRules(string validationRules)
        {
            validator = validationRules switch
            {
                CustomValidationRulesValue => new ValidatorBuilder().CreateCustom(),
                _ => new ValidatorBuilder().CreateDefault(),
            };

            currentValidationRules = validationRules;
        }

        private static void SetStorageRules(string storageRules)
        {
            switch (storageRules)
            {
                case "file":
                    FileStream fileStream = new (FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fileCabinetService = new FileCabinetFilesystemService(fileStream, validator);
                    break;

                default:
                    fileCabinetService = new FileCabinetMemoryService(validator);
                    break;
            }

            currentStorageRules = storageRules;
        }

        private static void SetUseRules(string useStopwatchRules)
        {
            switch (useStopwatchRules)
            {
                case "stopwatch":
                    isServiceMeterEnable = true;
                    break;

                case "logger":
                    isServiceLoggerEnable = true;
                    break;

                default:
                    break;
            }
        }

        private static void DefaultRecordPrint(IEnumerator<FileCabinetRecord> records)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            if (!records.MoveNext())
            {
                Console.WriteLine("Nothing found");
                return;
            }

            do
            {
                var record = records.Current;
                string date = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {date}, {record.Height} cm, {record.CashSavings}$, {record.FavoriteLetter}");
            }
            while (records.MoveNext());
        }
    }
}