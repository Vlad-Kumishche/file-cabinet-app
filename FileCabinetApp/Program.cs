using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using FileCabinetApp.CommandHandlers;

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
        private static string currentValidationRules = "default";
        private static string currentStorageRules = "memory";
        public static IRecordValidator validator = new DefaultValidator();
        public static bool IsRunning = true;

        public static IFileCabinetService FileCabinetService = new FileCabinetMemoryService(validator);

        private static Dictionary<string, SetRule> paramsList = new Dictionary<string, SetRule>
        {
            ["--storage"] = new SetRule(SetStorageRules),
            ["-s"] = new SetRule(SetStorageRules),
            ["--validation-rules"] = new SetRule(SetValidationRules),
            ["-v"] = new SetRule(SetValidationRules),
        };

        private delegate void SetRule(string args);

        /// <summary>
        /// Processes user input and calls the appropriate functions.
        /// </summary>
        /// <param name="args">Сommand line arguments.</param>
        public static void Main(string[] args)
        {
            ParseCommandLineParams(args);
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine($"Using {currentValidationRules} validation rules.");
            Console.WriteLine($"Using {currentStorageRules} to store records");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

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

                var commandHandler = CreateCommandHandlers();
                commandHandler.Handle(new AppCommandRequest(command, parameters));
            }
            while (IsRunning);
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            return new CommandHandler();
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
                    operation = args[first].Substring(0, index);
                    parameter = args[first].Substring(index + 1);
                    if (paramsList.ContainsKey(operation.ToLower(CultureInfo.InvariantCulture)))
                    {
                        changeRule = paramsList[operation.ToLower(CultureInfo.InvariantCulture)];
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

                if (paramsList.ContainsKey(operation.ToLower(CultureInfo.InvariantCulture)))
                {
                    changeRule = paramsList[operation.ToLower(CultureInfo.InvariantCulture)];
                }
            }

            if (changeRule != null && !string.IsNullOrEmpty(operation) && !string.IsNullOrEmpty(parameter))
            {
                changeRule.Invoke(parameter.ToLower(CultureInfo.InvariantCulture));
            }
        }

        private static void SetValidationRules(string validationRules)
        {
            switch (validationRules)
            {
                case "custom":
                    validator = new CustomValidator();
                    break;

                default:
                    break;
            }

            currentValidationRules = validationRules;
        }

        private static void SetStorageRules(string storageRules)
        {
            switch (storageRules)
            {
                case "file":
                    FileStream fileStream = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    FileCabinetService = new FileCabinetFilesystemService(fileStream, validator);
                    break;

                default:
                    break;
            }

            currentStorageRules = storageRules;
        }
    }
}