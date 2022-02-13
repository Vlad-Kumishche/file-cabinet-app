using System.Collections.ObjectModel;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for help command.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints statistics on records", "The 'stat' command prints statistics on records." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "insert", "inserts a new record with SQL-like syntax", $"The 'insert' command inserts a new record with SQL-like syntax. {Environment.NewLine}{Environment.NewLine}Examples:{Environment.NewLine}\tinsert (id, firsName, lastName, dateOfBirth, height, cashSavings, favoriteLetter) values (1, Denis, Villeneuve, 10/03/1967, 182, 250000, D){Environment.NewLine}\tinsert (firsName, lastName, dateOfBirth, height, cashSavings, favoriteLetter) values (Vlad, Who, 08/22/2002, 186, 300, F)" },
            new string[] { "update", "updates records with SQL-like syntax", $"The 'update' command updates records with SQL-like syntax. Supported logical operators: 'AND', 'OR'.{Environment.NewLine}{Environment.NewLine}Examples:{Environment.NewLine}\tupdate set firstname = 'John', lastname = 'Doe' where height = '182' and dateofbirth = '5/18/1986'{Environment.NewLine}\tupdate set FavoriteLetter = 'A' where FirstName = 'Tomas' or favoriteetter = 'f'{Environment.NewLine}\tupdate set CashSavings = '100' where *" },
            new string[] { "select", "selects records with SQL-like syntax", $"The 'select' command updates records with SQL-like syntax. Supported logical operators: 'AND', 'OR'.{Environment.NewLine}{Environment.NewLine}Examples:{Environment.NewLine}\tselect id, firstname, LastName where firstname = 'John' and lastname = 'Doe'{Environment.NewLine}\tselect * where FavoriteLetter = 'g' or height = '168'{Environment.NewLine}\tselect *" },
            new string[] { "export", "exports data to the file", $"The 'export' command exports the data to the <param1> file format located in the <param2> folder.{Environment.NewLine}{Environment.NewLine}Examples:{Environment.NewLine}\texport csv rec.csv{Environment.NewLine}\texport xml records.xml" },
            new string[] { "import", "imports data from the file", $"The 'import' command imports the data from the <param2> path in <param1> format.{Environment.NewLine}{Environment.NewLine}Examples:{Environment.NewLine}\timport csv rec.csv{Environment.NewLine}\timport xml records.xml" },
            new string[] { "delete", "deletes records with SQL-like syntax", $"The 'delete' command deletes records with SQL-like syntax. Supported logical operators: 'AND', 'OR'.{Environment.NewLine}{Environment.NewLine}Examples:{Environment.NewLine}\tdelete where firstName = 'Denis'{Environment.NewLine}\tdelete where firstName = 'Denis' and Height = '164'{Environment.NewLine}\tdelete *" },
            new string[] { "purge", "defragments the data file", "The 'purge' command defragments the data file." },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommandHandler"/> class.
        /// </summary>
        public HelpCommandHandler()
        {
            this.CommandName = "help";
        }

        /// <summary>
        /// Gets a list of commands.
        /// </summary>
        /// <returns>List of commands.</returns>
        public static ReadOnlyCollection<string> GetListOfCommands()
        {
            var commnads = new List<string>();
            foreach (var commands in HelpMessages)
            {
                commnads.Add(commands[0]);
            }

            return new (commnads);
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }

                Console.WriteLine();
                Console.WriteLine("For more details type 'help [commandName]'");
            }

            Console.WriteLine();
        }
    }
}
