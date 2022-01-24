using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Uladzislau Kumishcha";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;

        private static FileCabinetService fileCabinetService = new FileCabinetService();

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints statistics on records", "The 'stat' command prints statistics on records." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "list", "prints all records", "The 'list' command prints all records." },
            new string[] { "edit", "edits an existing record where id = <param1>. <param1> - id to search for", "The 'edit' command edits an existing record where id = <param1>. <param1> - id to search for." },
            new string[] { "find", "finds a list of records where <param1> = <param2>. <param1> - property name, <param2> - search text in quotes", "The 'edit' command finds a list of records where <param1> = <param2>. <param1> - property name, <param2> - search text in quotes." },
        };

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
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
            string? firstName;
            string? lastName;
            DateTime dateOfBirth;
            short height;
            decimal cashSavings;
            char favoriteLetter;

            InputRecord(out firstName, out lastName, out dateOfBirth, out height, out cashSavings, out favoriteLetter);

            int recordId = fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, height, cashSavings, favoriteLetter);
            Console.WriteLine($"Record #{recordId} is created.");
        }

        private static void InputRecord(out string? firstName, out string? lastName, out DateTime dateOfBirth, out short height, out decimal cashSavings, out char favoriteLetter)
        {
            const int minLength = 2;
            const int maxLength = 60;
            DateTime minDate = new DateTime(1950, 1, 1);
            const short minHeight = 40;
            const short maxHeight = 300;
            const decimal minCashSavings = 0M;
            const decimal maxCashSavings = 10_000_000M;

            firstName = InputOnlyLetters("First name: ", minLength, maxLength);
            lastName = InputOnlyLetters("Last name: ", minLength, maxLength);
            dateOfBirth = InputDate("Date of birth", minDate);
            height = InputNumber("Height (cm): ", minHeight, maxHeight);
            cashSavings = InputNumber("Cash savings ($): ", minCashSavings, maxCashSavings);
            favoriteLetter = char.Parse(InputOnlyLetters("Favorite char: ", 1, 1));
        }

        private static string InputOnlyLetters(string inputPrompt, int minLengh, int maxLength)
        {
            bool validationPassed;
            string line;
            do
            {
                validationPassed = true;
                Console.Write(inputPrompt);
                line = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrEmpty(line) || line.Length < minLengh || line.Length > maxLength)
                {
                    validationPassed = false;
                }
                else
                {
                    foreach (char c in line)
                    {
                        if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z'))
                        {
                            validationPassed = false;
                            break;
                        }
                    }
                }

                if (!validationPassed)
                {
                    Console.WriteLine($"Invalid input. Only latin letters are allowed. Min length: {minLengh}, max lenght: {maxLength}.");
                }
            }
            while (!validationPassed);

            return line;
        }

        private static DateTime InputDate(string inputPrompt, DateTime minDate)
        {
            bool validationPassed;
            const int dateLenght = 10;
            string? line;
            DateTime birthday = DateTime.MinValue;
            do
            {
                validationPassed = false;
                Console.Write(inputPrompt + " (MM/DD/YYYY):");
                line = Console.ReadLine();

                if (!string.IsNullOrEmpty(line) && line.Length == dateLenght)
                {
                    string toParse = line[3..6] + line[..3] + line[6..];
                    if (DateTime.TryParse(toParse, out birthday))
                    {
                        if (birthday >= minDate && birthday < DateTime.Now)
                        {
                            validationPassed = true;
                        }
                    }
                }

                if (!validationPassed)
                {
                    Console.WriteLine($"Invalid date. Min date: {minDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}, max date: today.");
                }
            }
            while (!validationPassed);

            return birthday;
        }

        private static short InputNumber(string inputPrompt, short minNumber, short maxNumber)
        {
            short number;
            bool validationPassed;
            do
            {
                validationPassed = false;
                Console.Write(inputPrompt);
                if (short.TryParse(Console.ReadLine(), out number) && number >= minNumber && number <= maxNumber)
                {
                    validationPassed = true;
                }

                if (!validationPassed)
                {
                    Console.WriteLine($"Invalid number. Min number: {minNumber}, max number: {maxNumber}.");
                }
            }
            while (!validationPassed);

            return number;
        }

        private static decimal InputNumber(string inputPrompt, decimal minNumber, decimal maxNumber)
        {
            decimal number;
            bool validationPassed;
            do
            {
                validationPassed = false;
                Console.Write(inputPrompt);
                if (decimal.TryParse(Console.ReadLine(), out number) && number >= minNumber && number <= maxNumber)
                {
                    validationPassed = true;
                }

                if (!validationPassed)
                {
                    Console.WriteLine($"Invalid number. Min number: {minNumber}, max number: {maxNumber}.");
                }
            }
            while (!validationPassed);

            return number;
        }

        private static void List(string parameters)
        {
            var records = fileCabinetService.GetRecords();
            ShowRecords(records);
        }

        private static void ShowRecords(FileCabinetRecord[] records)
        {
            foreach (var record in records)
            {
                string date = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {date}, {record.Height} cm, {record.CashSavings}$, {record.FavoriteLetter}");
            }
        }

        private static void Edit(string parameters)
        {
            int id;
            if (!int.TryParse(parameters, out id))
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

            string? firstName;
            string? lastName;
            DateTime dateOfBirth;
            short height;
            decimal cashSavings;
            char favoriteLetter;

            InputRecord(out firstName, out lastName, out dateOfBirth, out height, out cashSavings, out favoriteLetter);
            fileCabinetService.EditRecord(id, firstName, lastName, dateOfBirth, height, cashSavings, favoriteLetter);
        }

        private static void Find(string parameters)
        {
            var paramsArray = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var propertyName = paramsArray[0];
            var searchText = paramsArray[1];

            if (string.IsNullOrWhiteSpace(propertyName) || string.IsNullOrWhiteSpace(searchText) || searchText[0] != '\"' || searchText[^1] != '\"')
            {
                Console.WriteLine("Two parameters required. <param1> - property name, <param2> - search text in quotes");
                return;
            }

            propertyName = propertyName.ToLowerInvariant();
            searchText = searchText[1..^1];
            var records = propertyName switch
            {
                "firstname" => fileCabinetService.FindByFirstName(searchText),
                _ => null,
            };

            if (records is null)
            {
                Console.WriteLine("Invalid <param1> - property name");
                return;
            }

            ShowRecords(records);
        }
    }
}