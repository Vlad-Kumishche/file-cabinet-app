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
            new string[] { "insert", "inserts a new record with SQL syntax", "The 'insert' command inserts a new record with SQL syntax. Example 'insert' command: insert (id, firsName, lastName, dateOfBirth, height, cashSavings, favoriteLetter) values (1, Denis, Villeneuve, 10/03/1967, 182, 250000, D)" },
            new string[] { "list", "prints all records", "The 'list' command prints all records." },
            new string[] { "edit", "edits an existing record", "The 'edit' command edits an existing record where id = <param1>. <param1> - id to search for." },
            new string[] { "find", "finds a list of records matching the search text", "The 'find' command finds a list of records where <param1> = <param2>. <param1> - property name, <param2> - search text in quotes." },
            new string[] { "export", "exports data to the file", "The 'export' command exports the data to the <param1> file format located in the <param2> folder." },
            new string[] { "import", "imports data from the file", "The 'import' command imports the data from the <param1> path." },
            new string[] { "remove", "removes the record by id", "The 'remove' command removes the record by id." },
            new string[] { "purge", "defragments the data file", "The 'purge' command defragments the data file." },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommandHandler"/> class.
        /// </summary>
        public HelpCommandHandler()
        {
            this.CommandName = "help";
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
            }

            Console.WriteLine();
        }
    }
}
