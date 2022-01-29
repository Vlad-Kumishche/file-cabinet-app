using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Class provides user interaction.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Uladzislau Kumishcha";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private const string ValidationParameter = "--validation-rules=";
        private const string ShortValidationParameter = "-v";
        private const string DefaultValidation = "default";
        private const string CustomValidation = "custom";

        private static bool isRunning = true;

        private static bool isDefaultValidation = true;

        private static IFileCabinetService fileCabinetService = new FileCabinetService(new DefaultValidator());

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints statistics on records", "The 'stat' command prints statistics on records." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "list", "prints all records", "The 'list' command prints all records." },
            new string[] { "edit", "edits an existing record", "The 'edit' command edits an existing record where id = <param1>. <param1> - id to search for." },
            new string[] { "find", "finds a list of records matching the search text", "The 'edit' command finds a list of records where <param1> = <param2>. <param1> - property name, <param2> - search text in quotes." },
            new string[] { "export", "exports data to the file", "The 'export' command exports the data to the <param1> file format located in the <param2> folder." },
        };

        /// <summary>
        /// Function that processes user input and calls the appropriate functions.
        /// </summary>
        /// <param name="args">Сommand line arguments.</param>
        public static void Main(string[] args)
        {
            var message = ParseCommandLineArguments(args);
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(message);
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                var inputs = line != null ? line.Split(' ', 2) : new string[] { string.Empty, string.Empty };
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static string ParseCommandLineArguments(string[] args)
        {
            bool shortArgumentNotation = false;
            string message = $"Using {DefaultValidation} validation rules.";
            foreach (var argument in args)
            {
                if (string.Equals(argument, "-v", StringComparison.Ordinal))
                {
                    shortArgumentNotation = true;
                }

                if ((shortArgumentNotation && string.Equals(argument, CustomValidation, StringComparison.OrdinalIgnoreCase)) || (!shortArgumentNotation && string.Equals(argument, ValidationParameter + CustomValidation, StringComparison.OrdinalIgnoreCase)))
                {
                    fileCabinetService = new FileCabinetService(new CustomValidator());
                    message = $"Using {CustomValidation} validation rules.";
                    isDefaultValidation = false;
                    break;
                }
            }

            return message;
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            var recordToCreate = new RecordArgs();

            Console.Write("First name: ");
            recordToCreate.FirstName = ReadInput(StringConverter, NameValidator);

            Console.Write("Last name: ");
            recordToCreate.LastName = ReadInput(StringConverter, NameValidator);

            Console.Write("Date of birth: ");
            recordToCreate.DateOfBirth = ReadInput(DateConverter, DateValidator);

            Console.Write("Height (cm): ");
            recordToCreate.Height = ReadInput(ShortConverter, HeightValidator);

            Console.Write("Cash savings ($): ");
            recordToCreate.CashSavings = ReadInput(DecimalConverter, CashSavingsValidator);

            Console.Write("Favorite char: ");
            recordToCreate.FavoriteLetter = ReadInput(CharConverter, LetterValidator);

            int recordId = fileCabinetService.CreateRecord(recordToCreate);
            Console.WriteLine($"Record #{recordId} created.");
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input ?? string.Empty);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static Tuple<bool, string, string> StringConverter(string stringToConvert)
        {
            return Tuple.Create(true, string.Empty, stringToConvert);
        }

        private static Tuple<bool, string, DateTime> DateConverter(string stringToConvert)
        {
            const string requiredDateFormat = "MM/DD/YYYY";
            DateTime birthday = DateTime.MinValue;
            if (!string.IsNullOrEmpty(stringToConvert) && stringToConvert.Length == requiredDateFormat.Length)
            {
                string toParse = stringToConvert[3..6] + stringToConvert[..3] + stringToConvert[6..];
                if (DateTime.TryParse(toParse, out birthday))
                {
                    return Tuple.Create(true, string.Empty, birthday);
                }
            }

            return Tuple.Create(false, $"{stringToConvert} is not a date", birthday);
        }

        private static Tuple<bool, string, short> ShortConverter(string stringToConvert)
        {
            if (short.TryParse(stringToConvert, out var number))
            {
                return Tuple.Create(true, string.Empty, number);
            }

            return Tuple.Create(false, $"{stringToConvert} is not short number", number);
        }

        private static Tuple<bool, string, decimal> DecimalConverter(string stringToConvert)
        {
            if (decimal.TryParse(stringToConvert, out var number))
            {
                return Tuple.Create(true, string.Empty, number);
            }

            return Tuple.Create(false, $"{stringToConvert} is not decimal number", number);
        }

        private static Tuple<bool, string, char> CharConverter(string stringToConvert)
        {
            if (char.TryParse(stringToConvert, out var c))
            {
                return Tuple.Create(true, string.Empty, c);
            }

            return Tuple.Create(false, $"{stringToConvert} is not character", c);
        }

        private static Tuple<bool, string> NameValidator(string nameToValidate)
        {
            int minLength;
            int maxLength;
            if (isDefaultValidation)
            {
                minLength = 2;
                maxLength = 60;
            }
            else
            {
                minLength = 4;
                maxLength = 20;
            }

            if (string.IsNullOrEmpty(nameToValidate) || nameToValidate.Length < minLength || nameToValidate.Length > maxLength)
            {
                return Tuple.Create(false, $"Length of \"{nameToValidate}\" does not meet the requirements. Min. length = {minLength}, max. lenght = {maxLength}");
            }

            foreach (char c in nameToValidate)
            {
                var validationResult = LetterValidator(c);
                if (!validationResult.Item1)
                {
                    return Tuple.Create(false, validationResult.Item2);
                }
            }

            return Tuple.Create(true, string.Empty);
        }

        private static Tuple<bool, string> DateValidator(DateTime dateToValidate)
        {
            DateTime minDate;
            DateTime maxDate;
            if (isDefaultValidation)
            {
                minDate = new DateTime(1950, 1, 1);
                maxDate = DateTime.Now;
            }
            else
            {
                minDate = new DateTime(1940, 1, 1);
                const int ageOfMajority = 18;
                maxDate = DateTime.Now.AddYears(-ageOfMajority);
            }

            if (dateToValidate < minDate || dateToValidate >= maxDate)
            {
                return Tuple.Create(false, $"Invalid date. Min date: {minDate.ToString("MM / dd / yyyy", CultureInfo.InvariantCulture)}, max date: {maxDate.ToString("MM / dd / yyyy", CultureInfo.InvariantCulture)}.");
            }

            return Tuple.Create(true, string.Empty);
        }

        private static Tuple<bool, string> HeightValidator(short heightToValidate)
        {
            short minHeight;
            short maxHeight;
            if (isDefaultValidation)
            {
                minHeight = 40;
                maxHeight = 300;
            }
            else
            {
                minHeight = 120;
                maxHeight = 250;
            }

            if (heightToValidate < minHeight || heightToValidate > maxHeight)
            {
                return Tuple.Create(false, $"The height is not within the allowed range. Min. value = {minHeight}, max. value = {maxHeight}");
            }

            return Tuple.Create(true, string.Empty);
        }

        private static Tuple<bool, string> CashSavingsValidator(decimal heightToValidate)
        {
            decimal minCashSavings;
            decimal maxCashSavings;
            if (isDefaultValidation)
            {
                minCashSavings = 0M;
                maxCashSavings = 10_000_000M;
            }
            else
            {
                minCashSavings = 100M;
                maxCashSavings = 100_000_000M;
            }

            if (heightToValidate < minCashSavings || heightToValidate > maxCashSavings)
            {
                return Tuple.Create(false, $"The cash savings is not within the allowed range. Min. value = {minCashSavings}, max. value = {maxCashSavings}");
            }

            return Tuple.Create(true, string.Empty);
        }

        private static Tuple<bool, string> LetterValidator(char c)
        {
            if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z'))
            {
                return Tuple.Create(false, $"\"{c}\" is not an English letter");
            }

            return Tuple.Create(true, string.Empty);
        }

        private static void List(string parameters)
        {
            var records = fileCabinetService.GetRecords();
            ShowRecords(records);
        }

        private static void ShowRecords(ReadOnlyCollection<FileCabinetRecord> records)
        {
            foreach (var record in records)
            {
                string date = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {date}, {record.Height} cm, {record.CashSavings}$, {record.FavoriteLetter}");
            }
        }

        private static void Edit(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine($"#{parameters} record is not found.");
                return;
            }

            try
            {
                fileCabinetService.GetRecordById(id);
            }
            catch
            {
                Console.WriteLine($"#{parameters} record is not found.");
                return;
            }

            var recordToEdit = new RecordArgs();
            Console.Write("First name: ");
            recordToEdit.FirstName = ReadInput(StringConverter, NameValidator);

            Console.Write("Last name: ");
            recordToEdit.LastName = ReadInput(StringConverter, NameValidator);

            Console.Write("Date of birth: ");
            recordToEdit.DateOfBirth = ReadInput(DateConverter, DateValidator);

            Console.Write("Height (cm): ");
            recordToEdit.Height = ReadInput(ShortConverter, HeightValidator);

            Console.Write("Cash savings ($): ");
            recordToEdit.CashSavings = ReadInput(DecimalConverter, CashSavingsValidator);

            Console.Write("Favorite char: ");
            recordToEdit.FavoriteLetter = ReadInput(CharConverter, LetterValidator);

            recordToEdit.Id = id;
            fileCabinetService.EditRecord(recordToEdit);
            Console.WriteLine($"Record #{id} edited.");
        }

        private static void Find(string parameters)
        {
            const int paramsNumber = 2;
            string[] parameterExplanations = new string[] { "property name", "search text in quotes" };
            if (!GetParameters(paramsNumber, parameters, parameterExplanations, out var paramsArray))
            {
                return;
            }

            var propertyName = paramsArray[0].ToLowerInvariant();
            var searchText = paramsArray[1][1..^1];

            if (searchText[0] != '\"' || searchText[^1] != '\"')
            {
                Console.WriteLine($"<param2> - {parameterExplanations[1]}");
                return;
            }

            var records = propertyName switch
            {
                "firstname" => fileCabinetService.FindByFirstName(searchText),
                "lastname" => fileCabinetService.FindByLastName(searchText),
                "dateofbirth" => fileCabinetService.FindByDateOfBirth(searchText),
                _ => null,
            };

            if (records is null)
            {
                Console.WriteLine("Invalid <param1> - property name");
                return;
            }

            if (records.Count == 0)
            {
                Console.WriteLine("Nothing found");
                return;
            }

            ShowRecords(records);
        }

        private static bool GetParameters(int count, string sourceString, string[] parameterExplanations, out string[] paramsArray)
        {
            paramsArray = sourceString.Split(' ', count, StringSplitOptions.RemoveEmptyEntries);
            foreach (var parameter in paramsArray)
            {
                if (parameter is null)
                {
                    Console.Write($"{count} parameters required.");
                    int i = 0;
                    foreach (var explanation in parameterExplanations)
                    {
                        Console.Write($" <param{++i}> - {explanation}.");
                    }

                    return false;
                }
            }

            return true;
        }

        private static void Export(string parameters)
        {
            const int paramsNumber = 2;
            string[] parameterExplanations = new string[] { "file format", "path" };
            if (!GetParameters(paramsNumber, parameters, parameterExplanations, out var paramsArray))
            {
                return;
            }

            var fileFormat = paramsArray[0];
            var path = paramsArray[1];
            var file = new FileInfo(path);
            if (file.Exists)
            {
                Console.Write($"File is exist - rewrite {path}? [Y/n] ");
                char answer = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (answer != 'Y')
                {
                    return;
                }
            }

            try
            {
                var snapshot = fileCabinetService.MakeSnapshot();
                switch (fileFormat)
                {
                    case "csv":
                        var fileWriter = new StreamWriter(path, false, Encoding.UTF8);
                        snapshot.SaveToCsv(fileWriter);
                        fileWriter.Close();
                        Console.WriteLine($"All records are exported to file {path}");
                        break;
                    default:
                        Console.WriteLine($"<param1> - unsuppurted file format.");
                        break;
                }
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine($"Export failed: can't open file {path}");
            }
        }
    }
}