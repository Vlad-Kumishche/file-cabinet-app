﻿using FileCabinetApp.Data;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for find command.
    /// </summary>
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        /// <param name="printer">Used printer.</param>
        public FindCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> printer)
            : base(fileCabinetService)
        {
            this.CommandName = "find";
            this.printer = printer;
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            const int paramsNumber = 2;
            string[] parameterExplanations = new string[] { "property name", "search text in quotes" };
            if (!GetParameters(paramsNumber, parameters, parameterExplanations, out var paramsArray))
            {
                return;
            }

            string propertyName;
            string searchText;

            try
            {
                propertyName = paramsArray[0].ToLowerInvariant();
                searchText = paramsArray[1];
            }
            catch
            {
                Console.WriteLine($"Invalid parameters. <param1> - {parameterExplanations[0]}. <param2> - {parameterExplanations[1]}");
                return;
            }

            if (searchText[0] != '\"' || searchText[^1] != '\"')
            {
                Console.WriteLine($"<param2> - {parameterExplanations[1]}");
                return;
            }

            searchText = searchText[1..^1];

            var records = propertyName switch
            {
                "firstname" => this.fileCabinetService.FindByFirstName(searchText),
                "lastname" => this.fileCabinetService.FindByLastName(searchText),
                "dateofbirth" => this.fileCabinetService.FindByDateOfBirth(searchText),
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

            this.printer(records);
        }
    }
}