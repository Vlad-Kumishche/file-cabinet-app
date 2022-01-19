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
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints statistics on records", "The 'stat' command prints statistics on records." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "list", "prints all records", "The 'list' command prints all records." },
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
            var firstName = InputOnlyLetters("First name: ");
            var lastName = InputOnlyLetters("Last name: ");
            DateTime birthday = InputDate("Date of birth");

            Console.Write("Height (cm): ");
            short height;
            if (!short.TryParse(Console.ReadLine(), out height))
            {
                height = 0;
            }

            Console.Write("Cash savings ($): ");
            decimal cashSavings;
            if (!decimal.TryParse(Console.ReadLine(), out cashSavings))
            {
                cashSavings = 0;
            }

            Console.Write("Favorite char: ");
            var favoriteChar = Console.ReadKey().KeyChar;
            Console.WriteLine();
            int recordId = fileCabinetService.CreateRecord(firstName, lastName, birthday, height, cashSavings, favoriteChar);
            Console.WriteLine($"Record #{recordId} is created.");
        }

        private static string InputOnlyLetters(string inputPrompt)
        {
            bool validationPassed;
            string line;
            do
            {
                validationPassed = true;
                Console.Write(inputPrompt);
                line = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrEmpty(line))
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
                    Console.WriteLine("Invalid date.");
                }
            }
            while (!validationPassed);

            return line;
        }

        private static DateTime InputDate(string inputPrompt)
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
                        if (birthday < DateTime.Now)
                        {
                            validationPassed = true;
                        }
                    }
                }

                if (!validationPassed)
                {
                    Console.WriteLine("Invalid input. Only latin letters are allowed.");
                }
            }
            while (!validationPassed);

            return birthday;
        }

        private static void List(string parameters)
        {
            var records = fileCabinetService.GetRecords();
            foreach (var record in records)
            {
                string date = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {date}, {record.Height} cm, {record.CashSavings}$, {record.FavoriteChar}");
            }
        }
    }
}